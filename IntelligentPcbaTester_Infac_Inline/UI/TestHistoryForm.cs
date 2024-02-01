using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class TestHistoryForm : Form
    {
        private TestHistoryContext dbContext = null;
        private string prevOpenedFile = null;
        private int lastColumnWidth;

        public TestHistoryForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            groupDateColumn.GroupKeyGetter = rowObject =>
            {
                var history = rowObject as TestHistory;
                return history?.HistoryGroup?.StartTime;
            };
            groupDateColumn.GroupKeyToTitleConverter = groupKey =>
            {
                if (groupKey is DateTime startTime)
                {
                    return startTime.ToString("yyyy-MM-dd");
                }
                else
                {
                    return "";
                }
            };
            groupDateColumn.GroupFormatter = (group, parameters) =>
            {
                if (dbContext == null)
                {
                    return;
                }

                if (group.Key is DateTime startTime)
                {
                    var testGroup = dbContext.HistoryGroups.Where(g => g.StartTime == startTime).First();
                    if (testGroup != null)
                    {
                        double yield = FixtureProbeCount.CalcYieldPercent(testGroup.TestCount, testGroup.PassCount);
                        string subtitle = $"Total {testGroup.TestCount}, Pass {testGroup.PassCount}, Fail {testGroup.FailCount}, Yield {yield:0.0}%";
                        group.Subtitle = subtitle;
                    }
                }
            };
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            await ShowHistories();
        }

        private async Task ShowHistories()
        {
            var prevCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            try
            {
                dbContext?.Dispose();
                dbContext = new TestHistoryContext();
                await Task.Run(() =>
                {
                    var histories = dbContext.TestHistories.ToList();
                    for (int i = 0; i < histories.Count; i++)
                    {
                        histories[i].RowIndex = i + 1;
                    }
                    Utils.InvokeIfRequired(this, () =>
                    {
                        historyListView.SetObjects(histories);
                    });
                });
            }
            catch (Exception ex)
            {
                dbContext?.Dispose();
                dbContext = null;
                Utils.ShowErrorDialog(ex);
            }
            Cursor = prevCursor;
            Enabled = true;
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedCount = historyListView.SelectedObjects.Count;
            if (dbContext == null || selectedCount <= 0)
            {
                return;
            }

            // 한번 더 묻는다.
            var result = InformationBox.Show($"{selectedCount}개의 항목을 삭제하시겠습니까?", "삭제 확인",
                buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
            if (result != InformationBoxResult.OK)
            {
                return;
            }

            var prevCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    var selectedObjects = historyListView.SelectedObjects;
                    dbContext.TestHistories.RemoveRange(selectedObjects.Cast<TestHistory>());
                    dbContext.SaveChanges();
                    Utils.InvokeIfRequired(this, () =>
                    {
                        historyListView.RemoveObjects(selectedObjects);
                    });
                });
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
            Cursor = prevCursor;
            Enabled = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            dbContext?.Dispose();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            deleteToolStripMenuItem.Enabled = historyListView.SelectedObjects.Count > 0;
            showGroupsToolStripMenuItem.Checked = historyListView.ShowGroups;
            collapseGroupsToolStripMenuItem.Enabled = historyListView.OLVGroups?.Count > 0;
            expandGroupsToolStripMenuItem.Enabled = historyListView.OLVGroups?.Count > 0;
        }

        private void historyListView_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            // Double click만 처리.
            if (e.ClickCount != 2)
            {
                prevOpenedFile = null;
                return;
            }

            e.Handled = true;
            var displayedText = e.SubItem?.Text;

            if (!File.Exists(displayedText) || string.Equals(prevOpenedFile, displayedText, StringComparison.OrdinalIgnoreCase))
            {
                prevOpenedFile = null;
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(displayedText);
                prevOpenedFile = displayedText;
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
                prevOpenedFile = null;
            }
        }

        private void showGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            historyListView.BeginUpdate();
            if (!historyListView.ShowGroups && historyListView.LastSortColumn == noColumn)
            {
                historyListView.Unsort();
            }
            showGroupsToolStripMenuItem.Checked = historyListView.ShowGroups = !historyListView.ShowGroups;
            if (historyListView.ShowGroups)
            {
                lastColumnWidth = noColumn.Width;
                historyListView.Columns.Remove(noColumn);
            }
            else
            {
                noColumn.Width = lastColumnWidth;
                historyListView.Columns.Insert(0, noColumn);
            }
            historyListView.BuildList();
            historyListView.EndUpdate();
        }

        private async void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbContext == null)
            {
                return;
            }

            // 한번 더 묻는다.
            var result = InformationBox.Show($"검사이력을 초기화하시겠습니까?", "초기화 확인",
                buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
            if (result != InformationBoxResult.OK)
            {
                return;
            }

            var prevCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    dbContext.Database.ExecuteSqlCommand($"DELETE FROM {nameof(TestHistoryContext.TestHistories)}");
                    dbContext.Database.ExecuteSqlCommand($"DELETE FROM {nameof(TestHistoryContext.HistoryGroups)}");
                    Utils.InvokeIfRequired(this, () =>
                    {
                        historyListView.ClearObjects();
                    });
                });
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
            Cursor = prevCursor;
            Enabled = true;
        }

        private void collapseGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyListView.OLVGroups == null || historyListView.OLVGroups.Count == 0)
            {
                return;
            }
            historyListView.BeginUpdate();
            foreach (var group in historyListView.OLVGroups)
            {
                group.Collapsed = true;
            }
            historyListView.EndUpdate();
        }

        private void expandGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyListView.OLVGroups == null || historyListView.OLVGroups.Count == 0)
            {
                return;
            }
            historyListView.BeginUpdate();
            foreach (var group in historyListView.OLVGroups)
            {
                group.Collapsed = false;
            }
            historyListView.EndUpdate();
        }
    }
}
