/******************************************************************************
*
*	File Version: 3,0,1,0
*
*	Copyright(c) 2012 Alpha Motion Co,. Ltd. All Rights Reserved.
*
*	This file is strictly confidential and do not distribute it outside.
*
*	MODULE NAME :
*		pmiMApi.cs
*
*	AUTHOR :
*		K.C. Lee
*
*	DESCRIPTION :
*		the header file for RC files of project.
*
*
* - Phone: +82-31-303-5050
* - Fax  : +82-31-303-5051
* - URL  : http://www.alphamotion.co.kr,
*
*
/****************************************************************************/


using System.Runtime.InteropServices;

public class pmiMApi
{

	//***********************************************************************************************
	//		����̽� ����/���� �� �ʱ�ȭ
	//***********************************************************************************************

	//�ϵ���� ��ġ�� �ε��ϰ� �ʱ��մϴ�
	//��� �Լ��� ������ȣ ������ 0 ~ (���� �ý��ۿ� ������ ����� - 1) �������� ��ȿ
	//bManual = TRUE, Con Number is set to dip switch(Default)
	//nNumCons In a computer-set number of board
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSysLoad(int bManual, ref int npNumCons);

	//�ϵ���� ��ġ�� ��ε��մϴ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiSysUnload();

	//�ϵ���� �� ����Ʈ��� �ʱ�ȭ�մϴ�
	[DllImport("pmiMApi.dll")]
	public static extern int pmiConInit(int nCon);

    //========================================================================================================
    //                                   Motion Parameter Management public static extern int
    //========================================================================================================

    //��� Controller�� �������� ������ ���Ͽ��� �о� ����⿡ �����Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiConParamLoad(string szFilename);

    //���� ��� Controller�� ���� ��� �Ķ��Ÿ�� �� ���� .prm ���Ͽ� �����Ѵ�
    [DllImport("pmiMApi.dll")]
    public static extern int pmiConParamSave(string szFilename);    

	//============================================================================================
	//                       Motion interface I/O Configure and Control public static extern int
	//===========================================================================================

	//���� ���� Servo-On ��ȣ�� ����Ѵ�.
	//on=0, emOFF.
	//on=1, emON.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoOn(int nCon, int nAxis, int nState);

	//���� ���� Servo-On ��ȣ ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoOn(int nCon, int nAxis, ref int npState);

	//���� ���� Servo-Alarm Reset ��ȣ�� ����� �����Ѵ�.
	//nReset=0, emOFF.
	//nReset=1, emON.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoReset(int nCon, int nAxis, int nState);
                                            
	//���� ���� Servo-Alarm Reset ��ȣ�� ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoReset(int nCon, int nAxis, ref int npState);

	//���� ���� Inposition ��ȣ  Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoInpLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� Inpos ��ȣ Active Level ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoInpLevel(int nCon, int nAxis, ref int npLevel);
                                            
	//���� ���� Inpos ��ȣ ��� ���θ� �����Ѵ�.
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoInpEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� Inpos ��ȣ ��� ���θ� ��ȯ�Ѵ�.
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoInpEnable(int nCon, int nAxis, ref int npEnable);

	//���� ���� Inpos ��ȣ�� �Է� ���¸� ��ȯ�Ѵ�.
	//nInp=0, emINACTIVED.
	//nInp=1, emACTIVED.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoInp(int nCon, int nAxis, ref int npInp);

	//���� ���� �˶� ��ȣ�� Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoAlarmLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� �˶� ��ȣ�� Active Level ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoAlarmLevel(int nCon, int nAxis, ref int npLevel);

	//���� ���� �˶� ��ȣ �Է� �� ������ ����� �����Ѵ�.
	//nAction=0, emESTOP.
	//nAction=1, emSSTOP.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoAlarmAction(int nCon, int nAxis, int nAction);
                                            
	//���� ���� �˶� ��ȣ �Է½� ���� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoAlarmAction(int nCon, int nAxis, ref int npAction);

	//���� ���� ����� +/- ����Ƽ ���� ��ȣ Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetLimitLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� ����� +/- ����Ƽ ���� ��ȣ Active Level�� ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetLimitLevel(int nCon, int nAxis, ref int npLevel);

	//���� ���� ����� +/- ����Ƽ �Է� ��ȣ ���� �� ���� ����� �����Ѵ�.
	//nAction=0, emESTOP.
	//nAction=1, emSSTOP.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetLimitAction(int nCon, int nAxis, int nAction);
                                            
	//���� ���� ����� +/- ����Ƽ �Է� ��ȣ ���� �� ���� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetLimitAction(int nCon, int nAxis, ref int npAction);

	//���� ���� ���� �Է� ��ȣ Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetOrgLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� ���� ���� ��ȣ Active Level�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetOrgLevel(int nCon, int nAxis, ref int npLevel);
                                            
	//���� ���� Z�� �Է� ��ȣ Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetEzLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� Z�� ��ȣ Active Level ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetEzLevel(int nCon, int nAxis, ref int npLevel);

	//���� ���� ����Ʈ ����Ʈ ��ġ���� �����Ѵ�.
	//dLimitP = -134217728 ~ 134217727.
	//dLimitN = -134217728 ~ 134217727.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSoftLimitPos(int nCon, int nAxis, double dLimitP, double dLimitN);
                                            
	//���� ���� ����Ʈ ����Ʈ ��ġ���� ��ȯ�Ѵ�
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSoftLimitPos(int nCon, int nAxis, ref double dpLimitP, ref double dpLimitN);
                                            
	//���� ���� ����Ʈ ����Ʈ  +/- ����Ƽ �Է� ��ȣ ���� �� ���� ����� ��ȯ�Ѵ�.
	//nAction=0, �������� ����.
	//nAction=1, emESTOP.
	//nAction=2, emSSTOP.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSoftLimitAction(int nCon, int nAxis, int nAction);
                                            
	//���� ���� ����Ʈ ����Ʈ +/- ����Ƽ �Է� ��ȣ ���� �� ���� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSoftLimitAction(int nCon, int nAxis, ref int npAction);
                                            
	//���� ���� ����Ʈ ����Ʈ ��� ��� ������ �����Ѵ�.
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSoftLimitEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� ����Ʈ ����Ʈ ��� ��� ������ ��ȯ�Ѵ�
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSoftLimitEnable(int nCon, int nAxis, ref int npEnable);
                                            
	//���� ���� �� ī���� �ʱ�ȭ ��ġ���� �����Ѵ�
	//dPos = 0 ~ 134217727.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetRCountResetPos(int nCon, int nAxis, double dPos);
                                            
	//���� ���� �� ī���� �ʱ�ȭ ��ġ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetRCountResetPos(int nCon, int nAxis, ref double dpPos);

	//���� ���� �� ī���� ��� ��� ������ �����Ѵ�.
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetRCountEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� �� ī���� ��� ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetRCountEnable(int nCon, int nAxis, ref int npEnable);

	//���� ���� CRC(Current Remaining Clear) ��ȣ Active Level�� �����Ѵ�.
	//nLevel =0, emLOGIC_A.
	//nLevel =1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoCrcLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� CRC(Current Remaining Clear) ��ȣ Active Level�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoCrcLevel(int nCon, int nAxis, ref int npLevel);

	//���� ���� CRC(Current Remaining Clear)  ���� ��� ��ȣ �޽� �ð��� �����Ѵ�.
	//nOnTime =0, 12us.
	//nOnTime =1, 102us.
	//nOnTime =2, 408us.
	//nOnTime =3, 1.1ms.
	//nOnTime =4, 13ms.
	//nOnTime =5, 52ms.
	//nOnTime =6, 104ms.
	//nOnTime =7, Level
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoCrcTime(int nCon, int nAxis, int nOnTime);
                                            
	//���� ���� CRC(Current Remaining Clear)  ���� ��� ��ȣ �޽� �ð��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoCrcTime(int nCon, int nAxis, ref int npOnTime);

	//���� ���� EL, ALM, EMG �Է¿� ���� �� ���� ���� ���� CRC ��ȣ ��¿��θ� �����Ѵ�..
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoCrcEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� EL, ALM, EMG �Է¿� ���� �� ���� ���� ���� CRC ��ȣ ��¿��θ� ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetServoCrcEnable(int nCon, int nAxis, ref int npEnable);

	//���� ���� CRC ��ȣ�� ����Ʈ����� ON ��Ų��.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoCrcOn(int nCon, int nAxis);

	//���� ����CRC ��ȣ�� ����Ʈ����� OFF ��Ų��.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetServoCrcReset(int nCon, int nAxis);

    //Reset COUNTER when the CLR input turns OFF->ON.
    //Action = 0, Falling edge clear
    //Action = 1, Rising edge clear
    //Action = 2, Low Level clear
    //Action = 3, Hi Level clear
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCountAction(int nCon, int nAxis, int nAction);

    //Reset COUNTER when the CLR input turns OFF->ON.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCountAction(int nCon, int nAxis, ref int npAction);

	//���� ���� 4���� COUNTER(������ġ,����ġ,����,����) �ʱ�ȭ ��ų �Է� ��ȣ ��� ������ �����Ѵ�.
	//nReset=0, emFALSE.
	//nReset=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCountReset(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� COUNTER(������ġ,����ġ,����,����)�� �ʱ�ȭ ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCountReset(int nCon, int nAxis, ref int npEnable);

	//���� ���� SD(Start of Deceleration) ��ȣ Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSdLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ Active Level�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSdLevel(int nCon, int nAxis, ref int npLevel);

	//���� ���� SD(Start of Deceleration) ��ȣ �� �����Ѵ�.
	//nAction=0, emSDSLOW.
	//nAction=1, emSDSTOP.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSdAction(int nCon, int nAxis, int nStop);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSdAction(int nCon, int nAxis, ref int npStop);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ ON���� LATCH�� �����Ѵ�.
	//nLatch=0, emFALSE.
	//nLatch=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSdLatch(int nCon, int nAxis, int nLatch);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ ON���� LATCH�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSdLatch(int nCon, int nAxis, ref int npLatch);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ ��� ������ �����Ѵ�.
	//nEnable=0, emFALSE.
	//nEnable=1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSdEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� SD(Start of Deceleration) ��ȣ ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSdEnable(int nCon, int nAxis, ref int npEnable);

	//���� ���� PCS(Target Position Override) ��ȣ Active Level�� �����Ѵ�.
	//nLevel=0, emLOGIC_A.
	//nLevel=1, emLOGIC_B.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetPcsLevel(int nCon, int nAxis, int nLevel);
                                            
	//���� ���� PCS(Target Position Override) ��ȣ Active Level ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetPcsLevel(int nCon, int nAxis, ref int npLevel);
                                            
	//���� ���� PCS(Target Position Override) ��ȣ ��� ������ �����Ѵ�.
	//nEnable =0, emFALSE.
	//nEnable =1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetPcsEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� PCS(Target Position Override) ��ȣ ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetPcsEnable(int nCon, int nAxis, ref int npEnable);
                                          
	//���� ���� STA(Start simultaneously from an external circuit) Active ��ȣ�� �����Ѵ�.
	//nAction =0, emACT_LEVEL.
	//nAction =1, emACT_FALL.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetStaAction(int nCon, int nAxis, int nAction);
                                            
	//���� ���� STA(Start simultaneously from an external circuit) Active ��ȣ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetStaAction(int nCon, int nAxis, ref int npAction);

	//���� ���� STA(Start simultaneously from an external circuit) �Է� ��ȣ ��� ������ �����Ѵ�..
	//nEnable =0, emFALSE.
	//nEnable =1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetStaEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� STA(Start simultaneously from an external circuit) �Է� ��ȣ ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetStaEnable(int nCon, int nAxis, ref int npEnable);

    //���� ���� S/W command
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxStaBeginCmd(int nCon, int nAxis);

	//���� ���� STP(Stop simultaneously from an external circuit) ��ȣ ON �� �� ���� ����� �����Ѵ�.
	//nAction =0, emESTOP.
	//nAction =1, emSSTOP.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetStpAction(int nCon, int nAxis, int nAction);
                                            
	//���� ���� STP(Stop simultaneously from an external circuit) ��ȣ ON �� �� ���� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetStpAction(int nCon, int nAxis, ref int npAction);

	//���� ���� STP(Stop simultaneously from an external circuit) �Է� ��ȣ ��� ������ �����Ѵ�.
	//nEnable =0, emFALSE.
	//nEnable =1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetStpEnable(int nCon, int nAxis, int nEnable);
                                            
	//���� ���� STP(Stop simultaneously from an external circuit) �Է� ��ȣ ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetStpEnable(int nCon, int nAxis, ref int npEnable);

    //���� ���� S/W command
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxStpBeginCmd(int nCon, int nAxis);
                                            
	//���� ���� EA/EB/EZ �� PA/PB �Է� ��ȣ ���� ��� ������ �����Ѵ�.
	//nTarget =0, PA/PB
    //nTarget =1, EA/EB/EZ
	//nEnable =0, emFALSE.
	//nEnable =1, emTRUE.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetFilterABEnable(int nCon, int nAxis, int nTarget, int nEnable);
                                            
	//���� ���� EA/EB/EZ �� PA/PB �Է� ��ȣ ���� ��� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetFilterABEnable(int nCon, int nAxis, ref int nTarget, ref int npEnable);

    //nTarget =0, PA/PB
    //nTarget =1, EA/EB/EZ
    //���� ���� EA/EB/EZ �� PA/PB �Է� ��ȣ ���� �ð��� �����Ѵ�.
    //0000 : 20[ns]    0001 : 40[ns]    0010 : 60[ns]    0010 : 80[ns]
    //0011 : 100[ns]   0100 : 120[ns]   0101 : 140[ns]   0110 : 160[ns]
    //0111 : 180[ns]   1000 : 200[ns]   1001 : 220[ns]   1010 : 240[ns]
    //1100 : 260[ns]   1101 : 280[ns]   1110 : 300[ns]   1111 : 320[ns]
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetFilterABTime(int nCon, int nAxis, int nTarget, int nTime);

    //���� ���� EA/EB/EZ �� PA/PB �Է� ��ȣ ���� �ð��� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetFilterABTime(int nCon, int nAxis, ref int nTarget, ref int npTime);

	//���� ���� ���� ���� ��带 �����Ѵ�.
	//nCtrCount =0, emCOMM.
	//nCtrCount =1, emFEED.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCtrSetCount(int nCon, int nAxis, int nCount);

	//���� ���� ���� ��带 ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCtrGetCount(int nCon, int nAxis, ref int npCount);

	//���� ���� ���� �޽� ��� ����� �����Ѵ�.
	//nType =0, emONELOWHIGHLOW.	1�޽� ���, PULSE(Active High), ������(DIR=Low)  / ������(DIR=High)
	//nType =1, emONEHIGHHIGHLOW.	1�޽� ���, PULSE(Active High), ������(DIR=High) / ������(DIR=Low)
	//nType =2, emONELOWLOWHIGH.	1�޽� ���, PULSE(Active Low),  ������(DIR=Low)  / ������(DIR=High)
	//nType =3, emONEHIGHLOWHIGH.	1�޽� ���, PULSE(Active Low),  ������(DIR=High) / ������(DIR=Low)
	//nType =4, emTWOCWCCWLOW.		2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active High 
	//nType =5, emTWOCWCCWHIGH.		2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active Low
	//nType =6, emTWOCCWCWLOW.		2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active High
	//nType =7, emTWOCCWCWHIGH.		2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active Low
	//nType =8, emTWOPHASE.			2��(90' ������),  PULSE lead DIR(CW: ������), PULSE lag DIR(CCW:������)
	//nType =9, emTWOPHASERESERVE.  2��(90' ������),  PULSE lead DIR(CCW: ������), PULSE lag DIR(CW:������)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetPulseType(int nCon, int nAxis, int nType);

	//���� ���� ���� ��带 ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetPulseType(int nCon, int nAxis, ref int npType);

	//���� ���� �ǵ�� �޽� �Է� ����� �����Ѵ�.
	//nType =0, emEAB1X.   1ü��
	//nType =1, emEAB2X.   2ü��
	//nType =2, emEAB4X.   4ü��
	//nType =3, emCWCCW.   Up/Down
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetEncType(int nCon, int nAxis, int nType);

	//���� ���� �ǵ�� �޽� �Է� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetEncType(int nCon, int nAxis, ref int npType);

    //���� ���� �ǵ�� ī���� ������ �����Ѵ�.
    //0 normal counting(Default)
    //1 reverse counting
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetEncDir(int nCon, int nAxis, int nDir);

    //���� ���� �ǵ�� ī���� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetEncDir(int nCon, int nAxis, ref int npDir);

	//���� ���� �ǵ�� ī������ ī���� ������ �����Ѵ�.
	//nType =0, emCOMM.
	//nType =1, emFEED.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetEncCount(int nCon, int nAxis, int nCount);

	//���� ���� �ǵ�� ī������ ī���͸� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetEncCount(int nCon, int nAxis, ref int n��Count);

	//���� ���� �̼��Լ��� �ִ� �̼� �ӵ��� ������ �ӵ��� �����Ѵ�.
	//dVel = 0 ~ 6,553,500[pps].
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetMaxVel(int nCon, int nAxis, double dVel);

	//���� ���� �̼��Լ��� �ִ� �̼� �ӵ��� ������ �ӵ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetMaxVel(int nCon, int nAxis, ref double dpVel);

	//���� ���� ���� ���� �ӵ� �� ���� �ӵ��� �����Ѵ�.
	//dVel = 0 ~ 6,553,500[pps].
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetInitJogVel(int nCon, int nAxis, double dVel);

	//���� ���� ���� ���� �ӵ� �� ���� �ӵ��� ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetInitJogVel(int nCon, int nAxis, ref double dpVel);

    //���� ���� ���� �ӵ� �� ���� �ӵ��� �����Ѵ�.
    //dVel = 0 ~ 6,553,500[pps].
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetInitVel(int nCon, int nAxis, double dVel);

    //���� ���� �ʱ� �ӵ� �� ���� �ӵ��� ��ȯ�Ѵ�..
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetInitVel(int nCon, int nAxis, ref double dpVel);

    //���� �̼� �ӵ� ��������, �����ӵ�, ���ӵ��� �����Ѵ�.
    //nVelType = 0, emCONST.        �����Ӿ��� �ӵ� ��������
    //nVelType = 1, emTCURVE.       Trapezode �ӵ� ��������
    //nVelType = 2, emSCURVE.       S Curve   �ӵ� ��������
    //dVel     = 0 ~ 6,553,500[pps].
    //dTacc    = 0 ~ 65000[msec].
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetJogVelProf(int nCon, int nAxis, int nType, double dVel, double dTacc);

    //���� �̼� �ӵ� ��������, �����ӵ�, ���ӵ��� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetJogVelProf(int nCon, int nAxis, ref int npType, ref double dpVel, ref double dpTacc);

	//���� ���� �̼� �ӵ� ��������, �����ӵ�, ���ӵ� �� ���ӵ��� �����Ѵ�.
	//nVelType = 0, emCONST.        �����Ӿ��� �ӵ� ��������
	//nVelType = 1, emTCURVE.       Trapezode �ӵ� ��������
	//nVelType = 2, emSCURVE.       S Curve   �ӵ� ��������
	//dVel     = 0 ~ 6,553,500[pps].
	//dTacc    = 0 ~ 65000[msec].
	//dTdec    = 0 ~ 65000[msec].
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetVelProf(int nCon, int nAxis, int nType, double dVel, double dTacc, double dTdec);
                                               
	//���� ���� �̼� �ӵ� ��������, �����ӵ�, ���ӵ� �� ���ӵ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetVelProf(int nCon, int nAxis, ref int npType, ref double dpVel, ref double dpTacc, ref double dpTdec);


    //���� ���� ���� ��� ���Ͽ� �����Ѵ�.
    //nType = 0, AutoDetect.        �ڵ� ������ ����
    //nType = 1, ManualDetect.       ���� ������ ����
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetDecelType(int nCon, int nAxis, int nType);

    //���� ���� ���� ��� ���Ͽ� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetDecelType(int nCon, int nAxis, ref int npType);

    //���� ���� ���� ���� ��ġ�� ���Ͽ� �����Ѵ�.
    //dPul = -8,388,608 ~ 8,388,607[pulses]
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetRemainPul(int nCon, int nAxis, double dPulse);

    //���� ���� ���� ���� ��ġ�� ���Ͽ��� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetRemainPul(int nCon, int nAxis, ref double dpPulse);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetFilterEnable(int nCon, int nAxis, int nEnable);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetFilterEnable(int nCon, int nAxis, ref int nEnable);
    

	//========================================================================================================
	//                                   HOME-RETURN public static extern intS
	//========================================================================================================

	//���� ���� ���� �˻� �Ϸ� �� ī���� Reset�� �����ϴ� �Լ��̴�
	//nResetPos =0, �ϵ���� ��ȣ�� �ԷµǴ� ������ Command �� Feedback�� ��ġ�� 0 ���� ����. �������� Feedback ��ġ ������ ����.
	//nResetPos =1, ���� ���Ͱ� �Ϸ� �� �� Command �� Feedback�� ��ġ�� ��� �ڵ����� 0 ���� ����
	//nResetPos =2, ���� ���Ͱ� �Ϸ� �� �� Feedback ��ġ�� ������ ������ Command ��ġ�� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetResetPos(int nCon, int nAxis, int nResetPos);
                                               
	//���� ���� ���� �˻� �Ϸ� �� ī���� Reset�� ��ȯ�ϴ� �Լ��̴�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetResetPos(int nCon, int nAxis, ref int npResetPos);

    //Automatically outputs an CRC signal when the axis is stopped immediately by a ORG input signal. However, the CRC signal is not output when a
    //deceleration stop occurs on the axis. 
    //Even if the EL signal is specified for a normal stop, by setting MOD = "010X000" (feed to the EL position) in the RMD register, 
    //the CRC signal is output if an immediate stop occurs
    //crc_enable=0
    //crc_enable=1
    //TMC-BBxxP is not Supported a Function.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetCrcEnable(int nCon, int nAxis, int nEnable);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetCrcEnable(int nCon, int nAxis, ref int npEnable);

	//���� ���� �����˻��� �����ϱ� ���� ���� �˻� ����� �����Ѵ�.
    //TYPE=0 ORG ON -> Slow down -> Stop 
    //TYPE=1 ORG ON -> Stop -> Go back(Rev Spd) -> ORG OFF -> Go forward(Rev Spd) -> ORG ON -> Stop(Default)
    //TYPE=2 ORG ON -> Slow down(Low Spd) -> Stop on EZ signal
    //TYPE=3 ORG ON -> EZ signal -> Slow down -> Stop
    //TYPE=4 ORG ON -> Stop -> Go back(Rev Spd) -> ORG OFF -> Stop on EZ signal
    //TYPE=5 ORG ON -> Stop -> Go back(High Spd) -> ORG OFF -> EZ signal -> Slow down -> Stop
    //TYPE=6 EL ON -> Stop -> Go back(Rev Spd) -> EL OFF -> Stop
    //TYPE=7 EL ON -> Stop -> Go back(Rev Spd) -> EL OFF -> Stop on EZ signal
    //TYPE=8 EL ON -> Stop -> Go back(High Spd) -> EL OFF -> Stop on EZ signal
    //TYPE=9 ORG ON -> Slow down -> Stop -> Go back -> Stop at beginning edge of ORG
    //TYPE=10 ORG ON -> EZ signal -> Slow down -> Stop -> Go back -> Stop at beginning edge of EZ;
    //TYPE=11 ORG ON -> Slow down -> Stop -> Go back (High Spd) -> ORG OFF -> EZ signal -> Slow down -> Stop -> Go forward(High Spd) -> Stop at beginning edge of EZ
    //TYPE=12 EL ON -> Stop -> Go back (High Spd) -> EL OFF -> EZ signal -> Slow down -> Stop -> Go forward(High Spd) -> Stop at beginning edge of EZ
    //TYPE=13 EZ signal -> Slow down -> Stop
    [DllImport("pmiMApi.dll")]
	public static extern int pmiAxHomeSetType(int nCon, int nAxis, int nType);
                                               
	//���� ���� �����˻��� �����ϱ� ���� ���� �˻� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxHomeGetType(int nCon, int nAxis, ref int npType);

	//���� ���� �����˻��� �ʱ⿡ ���� �� ������ �����Ѵ�.
	//nDir =0, emDIR_P
	//nDir =1, emDIR_N
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetDir(int nCon, int nAxis, int nDir);
                                               
	//���� ���� �����˻��� �ʱ⿡ ���� �� ���⸦ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxHomeGetDir(int nCon, int nAxis, ref int npDir);

    //The origin of the origin of the behavior to escape from the distance sensor
    //pps ( -134217728 ~ 134217727 )
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetEscapePul(int nCon, int nAxis, double dEscape);

    //The origin of the origin of the behavior to escape from the distance sensor
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetEscapePul(int nCon, int nAxis, ref double dpEscape);

    //���� ���� �����˻� �� Ez���� ��� ī���� ���� �����Ѵ�.
    //nEzCount(1 ~ 16)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetEzCount(int nCon, int nAxis, int nCount);

    //���� ���� �����˻� �� Ez���� ��� ī���� ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetEzCount(int nCon, int nAxis, ref int npCount);

	//���� ���� �����˻��� �Ϸ�� �� �ⱸ �������� �̵� �� ���� �缳�� �� ��ġ�� �����Ѵ�.
	//dShift = 0 ~ 6553500(pulses)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetShiftDist(int nCon, int nAxis, double dShift);
                                               
	//���� ���� �����˻��� �Ϸ�� �� �ⱸ �������� �̵� �� ���� �缳�� �� ��ġ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxHomeGetShiftDist(int nCon, int nAxis, ref double dpShift);

	//���� ���� ���������� ������ �� �̼� �ӵ��� �����Ѵ�.
	//dVel = 0 ~ 6553500(pps)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetInitVel(int nCon, int nAxis, double dVel);
                                               
	//���� ���� ���������� ������ �� �̼� �ӵ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetInitVel(int nCon, int nAxis, ref double dpVel);

	//���� ���� ���� �˻� �ӵ� ��������, �����ӵ�, ������ӵ� �� ���ӵ��� �����Ѵ�.
	//nVelType = 0, emCONST.        �����Ӿ��� �ӵ� ��������
	//nVelType = 1, emTCURVE.       Trapezode �ӵ� ��������
	//nVelType = 2, emSCURVE.       S Curve   �ӵ� ��������
	//dVel     = 0 ~ 6,553,500[pps].
	//dRevVel  = 0 ~ 65000[msec].   ���� Ż�� �ӵ�
	//dTacc    = 0 ~ 65000[msec].
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetVelProf(int nCon, int nAxis, int nType, double dVel, double dRevVel, double dTacc);
                                               
	//���� ���� ���� �˻� �ӵ� ��������, �����ӵ�, ������ӵ� �� ���ӵ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetVelProf(int nCon, int nAxis, ref int npType, ref double dpVel, ref double dpRevVel, ref double dpTacc);
                                               
	//���� �࿡ ���� �˻� ���� �Լ��̴�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeMove(int nCon, int nAxis);
                                               
	//�� �࿡ ���� �˻� ���� �Լ��̴�.
	//nNAxis  �迭 ����
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxHomeMove(int nCon, int nNAxis, int[] naAxis);

	//���� �࿡ ���� �˻� ����� �Ϸ�ƴ����� ��ȯ�Ѵ�.
	//nDone =0, emSTAND
	//nDone =1, emRUNNING
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeCheckDone(int nCon, int nAxis, ref int npDone);

	//���� ���� ���� �˻� �Լ��� �̿��� ���� �˻��� ����ǰ� ���� �˻� ����� ���������� �����Ѵ�.
	//nStatus =0, emFALSE
	//nStatus =1, emTRUE
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeSetStatus(int nCon, int nAxis, int nStatus);
                                               
	//���� ���� ���� �˻� �Լ��� �̿��� ���� �˻��� ����ǰ� ���� �˻� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxHomeGetStatus(int nCon, int nAxis, ref int npStatus);

	//========================================================================================================
	//                                  Velocity mode And Single Axis Position Motion Configure
	//========================================================================================================

	//���� ���� ������ �������� ���� �̼��ϴ� �ӵ���� �Լ��̴�.
	//nDir =0, emDIR_P,    CW ����
	//nDir =1, emDIR_N,    CCW ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxJogMove(int nCon, int nAxis, int nDir);

	//���� ���� ������ ��ġ���� �̼� �Ѵ�
	//nAbsMode = 0, emINC   �����ǥ
	//nAbsMode = 1, emABS   ������ǥ
	//dPos     = -134217728 ~ 134217727[Pulses]  �̼� �Ÿ�
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxPosMove(int nCon, int nAxis, int nAbsMode, double dPos);

	//���� �࿡ ����� �Ϸ�ƴ����� ��ȯ�Ѵ�.
	//nDone =0, emSTAND
	//nDone =1, emRUNNING
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxCheckDone(int nCon, int nAxis, ref int npDone);
    
    //[DllImport("pmiMApi.dll")]
    //public static extern int pmiAxWaitDone(int nCon, int nAxis);    
                                               
	//���� ���� �̼� ���� ��� ������ ����ڰ� �� ���� �ð����� �����ϴ� �Լ��̴�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxStop(int nCon, int nAxis);
                                               
	//���� ���� ���� �̼� ���� ��� ������ ���� ���� �������ϴ� �Լ��̴�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxEStop(int nCon, int nAxis);
                                               
	//���� �࿡ ���Ͽ� �̼۸���� ���޵Ǿ��� �� �̼۵����� ������ �ٸ����� ���� ��Ȳ�� ����Ǿ� ���۵ǵ��� �����Ѵ�..
	//nType =0    �ٸ� �� ���� ������� ����.
	//nType =1,   �ٸ��� ���� ���� ��ȣ�� ���� ����.
	//nType =2    ������ �̼��� �Ϸ�� �� �̼� ����.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSyncType(int nCon, int nAxis, int nType);
                                               
	//���� �࿡ ���Ͽ� �̼۸���� ���޵Ǿ��� �� �̼۵����� ������ �ٸ����� ���� ��Ȳ�� ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSyncType(int nCon, int nAxis, ref int npType);
                                               
	//���� �࿡ ���Ͽ� �̼۸���� ���޵Ǿ��� �� �̼۵����� ������ �ٸ����� ���� ��Ȳ�� ����Ǿ� ���۵ǵ��� �����Ѵ�..
	//nType = 1    �� ��忡���� �� ���� �� ��ȣ�� �����Ѵ�. �� ������ ���� nAxis �Ű� ������ 0 ~ 3 ������ ���� ��쿡�� �� ���� 0 ~ 3 �̾�� �Ѵ�. �׸��� nAxis �Ű� ������ 4 ~ 7 ������ ���� ��쿡�� �� ���� 4 ~ 7 �̾�� �Ѵ�.
	//nType = 2    �� ��忡���� �� ���� �� ����Ʈ�� �����Ѵ�. �� ��쿡�� ���� ���� ���� �� ���� �� �� ������, �� ��Ʈ���� ���� 1�� ��� �ش����� �����ȴ�.
	//             ���� ��� emAXIS0, emAXIS2�� ������ ��� �����ϴ� ������ ����ϰ��� �Ѵٸ� nMaskAxes���� 0x5�� �����Ѵ�. �� ������ ���� nAxis �Ű� ������ 0 ~ 3 ������ ���� ��쿡�� nMaskAxes ���� BIT0 ~ BIT3 �� ����� �� ������, nAxis �Ű� ������ 4 ~ 7 ������ ���� ��쿡�� nMaskAxes ���� BIT4 ~ BIT7 �� ����� �� �ִ�.

	//nCondition =0    ���� ���� ��ȣ ��� OFF
	//nCondition =1    �������� ������ ���� �� �� �̼� ����
	//nCondition =2    �������� ������ ������ ���� ���� �� �� �̼� ����
	//nCondition =3    �������� ������ ���� �� �� �̼� ����
	//nCondition =4    �������� ������ ���� �� �̼� ����
	//nCondition =5    -SL ��ȣ�� ����Ǿ��� �� �̼� ����
	//nCondition =6    +SL ��ȣ�� ����Ǿ��� �� �̼� ����
	//nCondition =7    ���� �񱳱⿡ ������ ������ �����Ǿ��� �� �̼� ����
	//nCondition =8    TRG-CMP ������ �����Ǿ��� �� �̼� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSyncAction(int nCon, int nAxis, int nMaskAxes, int nCondition);
                                               
	//���� �࿡ ���Ͽ� �̼۸���� ���޵Ǿ��� �� �̼۵����� ������ �ٸ����� ���۸� ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSyncAction(int nCon, int nAxis, int nMaskAxes, ref int npContion);
                                               
	//========================================================================================================
	//                                  Velocity mode And Multi Axis Point to Point Motion Configure
	//========================================================================================================
                                              
	//�� ���� ������ �������� ���� �̼��ϴ� �ӵ���� �Լ��̴�.
	//nNAxis  �迭 ����
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	//naDir   �̼� ���� �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	//nDir = 0, emDIR_P,    CW ����
	//nDir = 1, emDIR_N,    CCW ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxJogMove(int nCon, int nNAxis, int[] naAxis, int[] naDir);

	//�� ���� ������ �������� ���� �̼��ϴ� �ӵ���� �Լ��̴�.
	//nNAxis  �迭 ����
	//nAbsMode =0, emINC. �����ǥ
	//nAbsMode =1, emABS. ������ǥ
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	//daDist  �̼� �Ÿ�(Pulses) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxPosMove(int nCon, int nNAxis, int nAbsMode, int[] naAxis, double[] daDist);
                                               
	//�� �࿡ ���ؼ� ����� �Ϸ�ƴ����� ��ȯ�Ѵ�
	//nNAxis  �迭 ����
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	//nDone =0, emSTAND
	//nDone =1, emRUNNING
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxCheckDone(int nCon, int nNAxis, int[] naAxis, ref int npDone);
                                               
	//�� �࿡ ���ؼ� �̼� ���� ��� ������ ���� �����ϴ� �Լ��̴�.
	//nNAxis  �迭 ����
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxStop(int nCon, int nNAxis, int[] naAxis);
                                               
	//�� �࿡ ���ؼ� �̼� ���� ��� ������ �������ϴ� �Լ��̴�.
	//nNAxis  �迭 ����
	//naAxis  �� ��ȣ (0 ~ (�ִ���� - 1)) �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
	public static extern int pmiMxEStop(int nCon, int nNAxis, int[] naAxis);
                                              
	//========================================================================================================
	//                                  Motion I/O Monitoring public static extern intS
	//========================================================================================================

	//���� ���� �ܺ� ���� �� ���� ���� ��ȣ���� ���¸� ��ȯ�Ѵ�..
	//BIT0    �������(EMG) ��ȣ �Է� ����
	//BIT1    Alarm ��ȣ �Է� ����
	//BIT2    +EL ���� ��ȣ �Է� ����
	//BIT3    -EL ���� ��ȣ �Է� ����
	//BIT4    ���� ��ȣ ����
	//BIT5    �޽� ��� ���� ����(  0 : +����, - : -���� )
	//BIT6    ���� �˻� �Ϸ� ���� ����
	//BIT7    PCS(Position Override) ��ȣ �Է� ����
	//BIT8    CRC ��ȣ �Է� ����
	//BIT9    Z�� ��ȣ �Է� ����
	//BIT10   CLR �Է� ��ȣ ����
	//BIT11   LATCH(Position Latch) ��ȣ �Է� ����
	//BIT12   SD(Slow Down) ��ȣ �Է� ����
	//BIT13   Inpos ��ȣ �Է� ����
	//BIT14   ���� �� ��ȣ �Է� ����
	//BIT15   �˶� ���� ��ȣ �Է� ����
	//BIT16   STA ��ȣ �Է� ����
	//BIT17   STP ��ȣ �Է� ����
	//BIT18   SYNC Pos Error signal input
	//BIT19   GANT Pos Erorr signal input
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetMechanical(int nCon, int nAxis, ref int npMechanical);
                                               
	//���� ���� ��� �̼� ���¸� ��ȯ�Ѵ�.
	//BIT0    ���� ���� ��
	//BIT1    �ܺ� ����ġ ��ȣ ��ٸ�
	//BIT2    ���� ���� ��ȣ ��ٸ�
	//BIT3    ���� ���� ��ȣ ��ٸ�
	//BIT4    Ÿ�� ���� ��ȣ ��ٸ�
	//BIT5    CRC ��� �Ϸ� ��ٸ�
	//BIT6    ���� ��ȭ ��ٸ�
	//BIT7    �鷡�� ����
	//BIT8    PA/PB ��ȣ ��ٸ�
	//BIT9    ���� �˻� �ӵ� �̼� ��
	//BIT10   ���� �ӵ� �̼� ��
	//BIT11   ���� ��
	//BIT12   �۾� �ӵ� �̼� ��
	//BIT13   ���� ��
	//BIT14   InPos ��ȣ ��ٸ�
	//BIT15   Reserved
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetMotion(int nCon, int nAxis, ref int npMotion);

    //BIT0 EMG Error
    //BIT1 ALM Alarm Signal Error
    //BIT2 +EL Positive Limit Switch Error
    //BIT3 -EL Negative Limit Switch Error
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetErrStatus(int nCon, int nAxis, ref int npErrStatus);
                                                                                              
	//���� ���� ���� �̼� ���� �ӵ��� �о�´�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCmdVel(int nCon, int nAxis, ref double dpVel);
                                               
	//���� ���� ���� �ǵ�� �ӵ��� �о�´�.
	//�� �Լ��� ����ҷ��� pmiGnSetCheckActSpeed �Լ��� ����ؾ��Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetActVel(int nCon, int nAxis, ref double dpVel);
                                               
	//���� ���� ���� ��ġ�� �����Ѵ�.
	//dPos = -134,217,727 ~ +134,217,727
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCmdPos(int nCon, int nAxis, double dPos);
                                               
	//���� ���� ���� ��ġ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetCmdPos(int nCon, int nAxis, ref double dpPos);
                                               
	//���� ���� �ǵ�� ��ġ�� �����Ѵ�.
	//dPos = -134,217,727 ~ +134,217,727
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxSetActPos(int nCon, int nAxis, double dPos);
                                               
	//���� ���� �ǵ�� ��ġ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetActPos(int nCon, int nAxis, ref double dpPos);
                                               
                                               
	//���� ���� ������ġ(Command)�� �ǵ����ġ(Encoder)�� ������ �����Ѵ�.
	//dErrPos = -32,768 ~ +32,767   ���� ī����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetPosError(int nCon, int nAxis, double dErrPos);
                                               
	//���� ���� ������ġ(Command)�� �ǵ����ġ(Encoder)�� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetPosError(int nCon, int nAxis, ref double dpErrPos);
                                               
	//���� ���� ���� ī���� ���� �߻��� �����Ѵ�.
	//dMethod
	//emMTH_NONE      0    �������� ����
	//emMTH_EQ_DIR    1    Counter ����� ����
	//emMTH_EQ_PDIR   2    Counter Up ��
	//emMTH_EQ_NDIR   3    Counter Down ��
	//dAction
	//emEVT_ONLY      0    ó�� ���� ����
	//emEVT_ESTOP     1    ��� ����
	//emEVT_STOP      2    ���� ����
	//emEVT_SPDCHG    3    �ӵ� ����
	//dPos =-134,217,727 ~ +134,217,727[pulses] ���� ī����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetPosErrAction(int nCon, int nAxis, int nMethod, int nAction, double dPos);
                                               
	//���� ���� ���� ī���͸� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetPosErrAction(int nCon, int nAxis, ref int npMethod, ref int npAction, ref double dpPos);
                                               
	//���� ���� ���� �� ī���͸� �����Ѵ�.
	//emCOMM  0    ���� ��ġ ī����
	//emFEED  1    ���� ��ġ ī����
	//emDEV   2    ���� ī����
	//emGEN   3    ���� ī����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetGenSource(int nCon, int nAxis, int nCounter);
                                               
	//���� ���� ���� �� ī���͸� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiAxGetGenSource(int nCon, int nAxis, ref int npCounter);
                                               
	//���� ���� ���� ī���� ���� �߻��� �����Ѵ�.
	//dMethod
	//emMTH_NONE      0    �������� ����
	//emMTH_EQ_DIR    1    Counter ����� ����
	//emMTH_EQ_PDIR   2    Counter Up ��
	//emMTH_EQ_NDIR   3    Counter Down ��
	//dAction
	//emEVT_ONLY      0    ó�� ���� ����
	//emEVT_ESTOP     1    ��� ����
	//emEVT_STOP      2    ���� ����
	//emEVT_SPDCHG    3    �ӵ� ����
	//dPos =-134,217,727 ~ +134,217,727 ���� ī����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetGenAction(int nCon, int nAxis, int nMethod, int nAction, double dPos);
                                               
	//���� ���� ���� �� ī���� ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetGenAction(int nCon, int nAxis, ref int npMethod, ref int npAction, ref double dpPos);

    //This function is select a auto speed change method to use when the comparator conditions are satisfied. 
    //emCOMM  0    ���� ��ġ ī����
    //emFEED  1    ���� ��ġ ī����
    //emDEV   2    ���� ī����
    //emGEN   3    ���� ī����
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCmpModifySource(int nCon, int nAxis, int nCounter);

    //
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCmpModifySource(int nCon, int nAxis, ref int npCounter);

    //nMethod
    //emMTH_NONE      0    �������� ����
    //emMTH_EQ_DIR    1    Counter ����� ����
    //emMTH_EQ_PDIR   2    Counter Up ��
    //emMTH_EQ_NDIR   3    Counter Down ��
    //dAction
    //emEVT_ONLY      0    ó�� ���� ����
    //emEVT_ESTOP     1    ��� ����
    //emEVT_STOP      2    ���� ����
    //emEVT_SPDCHG    3    �ӵ� ����
    //dPos =-134,217,727 ~ +134,217,727 ���� ī����
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCmpModifyAction(int nCon, int nAxis, int nMethod, int nAction, double dPos);

    //���� ���� ���� �� ī���� ���� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCmpModifyAction(int nCon, int nAxis, ref int npMethod, ref int npAction, ref double dpPos);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetCmpModifyVel(int nCon, int nAxis, double dVel);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetCmpModifyVel(int nCon, int nAxis, ref double dpVel);

	//========================================================================================================
	//                                  Overriding public static extern intS
	//========================================================================================================

	//���� ���� ��� ���� �� ������ �̵� �Ÿ��� �������̵� �Ѵ�.
    //dPos = -134,217,728 <= new pos <= 134,217,727
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxModifyPos(int nCon, int nAxis, double dPos);

    //���� ���� ��� ���� �� ������ �̵� �Ÿ��� �������̵� �Ѵ�.
    //dPos = -134,217,728 <= new pos <= 134,217,727
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMxModifyPos(int nCon, int nNAxis, int[] naAxes, double[] daPos);

    //���� ���� ��� ���� �� ������ �̼� �ӵ��� �������̵� �Ѵ�.
    //dOvr = 1 ~ 6553500
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxModifyVel(int nCon, int nAxis, double dOvr);

    //���� ���� ��� ���� �� ������ �̼� �ӵ��� �������̵� �Ѵ�.
    //dOvr = 1 ~ 6553500
    [DllImport("pmiMApi.dll")]
    public static extern int pmiMxModifyVel(int nCon, int nNAxis, int[] naAxes, double[] daOvr);
    
    //���� ���� ��� ���� �� ������ �̼� �ӵ��� Ÿ�Ե��� �����Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxModifyVelProf(int nCon, int nAxis, double dVel, double dTacc, double dTdec);
 	
	//========================================================================================================
	//                                  Coordinat Motion Control
	//========================================================================================================

	//���� ��� �� �׷��� �����Ѵ�...
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//nNAxis    : ���� ���� ������ ����
	//naAxis    : ���� �� ���� �迭(nNAxis���� ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
	public static extern int pmiCsSetAxis(int nCon, int nCs, int nNAxis, int[] naAxis);
                                               
	//���� ��� �� ���� �ӵ� �� ���� �ӵ���  �����Ѵ�...
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//dVel      : 0 ~ 6553500
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsSetInitVel(int nCon, int nCs, double dVel);

	//���� ��� �� ���� �ӵ� �� ���� �ӵ���  ��ȯ�Ѵ�...
	//nCs In    : ���� �ε��� ��ȣ( 0 ~ 3 )
	//dVel      : 0 ~ 6553500
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsGetInitVel(int nCon, int nCs, ref double dpVel);

	//���� ��� �� �̼� �ӵ��� �����Ѵ�.
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//nOpType  = 0,  emCNS_VECTOR.    ���� ���ǵ�
	//         = 1,  emCNS_MASTER.   ������ ���ǵ�
	//nVelType = 0,  emCONST.        �����Ӿ��� �ӵ� ��������
	//nVelType = 1,  emTCURVE.       Trapezode �ӵ� ��������
	//nVelType = 2,  emSCURVE.       S Curve   �ӵ� ��������
	//dVel    : 0 ~ 6553500 �۾� �ӵ�(pps) - ���� ���ǵ� Ÿ�� �� ���� PPS ������ �����Ѵ�
	//          0 ~ 100 �۾� �ӵ� ����(%)  - ������ ���ǵ� Ÿ�� �� ���� �۾� �ӵ� ����(%)�� �����Ѵ�.
	//dTacc   : 0 ~ 65000   ���ӵ�(msec)   - ���� ���ǵ� Ÿ�� �� ���� PPS ������ �����Ѵ�
	//          0 ~ 100     ���ӵ� ����(%) - ������ ���ǵ� Ÿ�� �� ���� �۾� �ӵ� ����(%)�� �����Ѵ�.
	//dTdec   : 0 ~ 65000   ���ӵ�(msec)   - ���� ���ǵ� Ÿ�� �� ���� PPS ������ �����Ѵ�
	//          0 ~ 100     ���ӵ� ����(%) - ������ ���ǵ� Ÿ�� �� ���� �۾� �ӵ� ����(%)�� �����Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsSetVelProf(int nCon, int nCs, int nOpType, int nType, double dVel, double dTacc, double dTdec);

	//���� ��� �� �̼� �ӵ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsGetVelProf(int nCon, int nCs, int nOpType, ref int npType, ref double dpVel, ref double dpTacc, ref double dpTdec);

	//���� �̼� ������ ���� �Ѵ�.
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//nAbsMode  : emINC   0    �����ǥ
	//          : emABS   1    ������ǥ
	//daPos     : �̼� �Ÿ� �迭
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsLinMove(int nCon, int nCs, int nAbsMode, double[] daPos);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsLinMoveEx(int nCon, int nCs, int nNAxis, int[] naAxis, int nAbsMode, double[] daPos);
                                               
	//��ȣ ���� �̼�(�߽� ��ǥ �� ���� ��ǥ)�� ���� �Ѵ�.
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//nAbsMode  : emINC   0    �����ǥ
	//          : emABS   1    ������ǥ
	//daCen     : �߽� ��ǥ �迭
	//daPos     : ���� ��ǥ �迭
	//nDir      : emARC_CW   0  - �ð����(CW)���� ȸ��
	//            emARC_CCW  1  - �ݽð����(CCW)���� ȸ��
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsArcPMove(int nCon, int nCs, int nAbsMode, double[] daCen, double[] daPos, int nDir);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsArcPMoveEx(int nCon, int nCs, int nNAxis, int[] naAxis, int nAbsMode, double[] daCen, double[] daPos, int nDir);


	//��ȣ ���� �̼�(�߽� ��ǥ �� ����) �� ���� �Ѵ�.
	//nCs       : ���� �ε��� ��ȣ( 0 ~ 3 )
	//nAbsMode  : emINC   0    �����ǥ
	//          : emABS   1    ������ǥ
	//daCen     : �߽� ��ǥ �迭
	//dAngle    : 0 ~  360 ������ ��ȣ(-)  - �ð����(CW)���� ȸ��
	//          : 0 ~  360 ������ ��ȣ(+)  - �ݽð����(CCW)���� ȸ��
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsArcAMove(int nCon, int nCs, int nAbsMode, double[] daCen, double dAngle);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsArcAMoveEx(int nCon, int nCs, int nNAxis, int[] naAxis, int nAbsMode, double[] daCen, double dAngle);

    //�︮�� ����.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsHelMove(int nCon, int nCs, double dCenX, double dCenY, double dPosX, double dPosY, double dPosZ, int nDir);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsHelMoveEx(int nCon, int nCs, int nNAxis, int[] naAxis, double dCenX, double dCenY, double dPosX, double dPosY, double dPosZ, int nDir);

    //���� �̼� ���ؼ� ����� �Ϸ�ƴ����� ��ȯ�Ѵ�
    //nDone         :  emSTAND     0    �̼� ���� ����
    //              :  emRUNNING   1    �̼� ��
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsCheckDone(int nCon, int nCs, ref int npDone);            
                           
	//���� �̼� ���� ������ �����Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsStop(int nCon, int nCs);
                                               
	//���� �̼� �� ������ �����Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCsEStop(int nCon, int nCs);                                            
	
	//========================================================================================================
	//                                  Continuous Motion
	//========================================================================================================

    //���� ���� ������ ���� ����� ���� Queue�� ��� �����ϴ� �Լ��̴�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContClearQueue(int nCon);

    //������ ��ǥ�迡 ���Ӻ������� ������ �۾����� ����� �����Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContBeginQueue(int nCon);

    //������ ��ǥ�迡�� ���Ӻ����� ������ �۾����� ����� �����Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContEndQueue(int nCon);

    //����� ���� ���� ���� Queue�� ������ �����ϴ� �Լ��̴�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContMove(int nCon);

    //����� ���� ���� ���� Queue�� ������ �����ϴ� �Լ��̴�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContStop(int nCon);

    //���� ���� ���� �� ���� �������� ���� ���� �ε��� ��ȣ�� Ȯ���ϴ� �Լ��̴�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCsContGetCurIndex(int nCon, int nLsi, ref int npIndex);

	//========================================================================================================
	//                                  Position Compare public static extern intS
	//========================================================================================================

    //Position Trigger ��� ��ȣ Active Level�� �����Ѵ�.
    //nCmp           : 0    �񱳱�[0]
    //               : 1    �񱳱�[1]
    //nLevel         : 0    emLOGIC_A   A����(NORMAL OPEN) �� Active Low Level Trigger
    //               : 1    emLOGIC_B   B����(NORMAL CLOSE) �� Active High Level Trigger
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCmpSetLevel(int nCon, int nCmp, int nLevel);

    //Position Trigger ��� ��ȣ Active Level�� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiCmpGetLevel(int nCon, int nCmp, ref int npLevel);

	//Position Trigger�� ����� �ฦ �����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	[DllImport("pmiMApi.dll")]
	public static extern int pmiCmpSetAxis(int nCon, int nCmp, int nAxis);

	//Position Trigger�� ����� �ฦ ��ȯ�Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	[DllImport("pmiMApi.dll")]
	public static extern int pmiCmpGetAxis( int nCon, int nCmp, ref int npAxis);	
                                               
	//Position Trigger ��� ��ȣ �޽� ���� �����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	//nPul           : 1 ~ 50000(Pulses)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpSetHoldTime(int nCon, int nCmp, int nPulse);
                                               
	//Position Trigger ��� ��ȣ �޽� ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpGetHoldTime(int nCon, int nCmp, ref int npPulse);

    //Position Trigger ��� ��ȣ�� ����� ������ ��ġ���� �� ���� Ʈ���� �޽��� ����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	//dPos           :
	[DllImport("pmiMApi.dll")]
	public static extern int pmiCmpSetSinglePos(int nCon, int nCmp, int nMethod, double dPos);
                                               
	//Position Trigger ��� ��ȣ�� ����� ������ ��ġ �������� Ʈ���� �޽��� ����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	//nMethod        : 0    emEQ_PDIR   - Counting up ��
	//               : 1    emEQ_NDIR   - Counting down ��
    //dNPos          :  -134217728 ~ +134217727    Ʈ���� ��� ���� ��ġ
    //dPPos          :  -134217728 ~ +134217727    Ʈ���� ��� ���� ��ġ
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpSetRangePos(int nCon, int nCmp, int nMethod, double dNPos, double dPPos);

	//Position Trigger ��� ��ȣ�� ����� ������ ������ġ���� ������ġ���� ������������ Ʈ���� ����� �����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	//nMethod        : 0    emEQ_PDIR   - Counting up ��
	//               : 1    emEQ_NDIR   - Counting down ��
	//nNum           : 0 ~ 1024                    - ����� ����
	//dSPos          :  -134217728 ~ +134217727    - Ʈ���� ��� ���� ��ġ
	//dDist          : 0 ~ +134217727              - Ʈ���� ��� �ֱ� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpSetMultPos(int nCon, int nCmp, int nMethod, int nNum, double dSPos, double dDist);

	//Position Trigger ��� ��ȣ�� ����� ������ ������ġ���� ������ġ���� ������������ Ʈ���� ����� �����Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	//nMethod        : 0    emEQ_PDIR  - Counting up ��
	//               : 1    emEQ_NDIR  - Counting down ��
	//nNum           : 0 ~ 1024        - ����� ����(�迭 ����)
	//daPos          :                 -Ʈ���� ��� ��ġ�迭(nNum ������ �������� ���ų� ũ�� �����ؾߵ�)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpSetPosTable(int nCon, int nCmp, int nMethod, int nNum, double[] daPos);

	//Position Trigger ��� ��ȣ Ʈ���Ÿ� �����Ѵ�.
	//nCmp           : 0   �񱳱�[0]
	//               : 1    �񱳱�[1]
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpBegin(int nCon, int nCmp);

	//Position Trigger ��� ��ȣ Ʈ���Ÿ� ��ü�Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpEnd(int nCon, int nCmp);

	//Position Compare Trigger ��ȣ ��� �߻� �� ��ġ���� ��ȯ�Ѵ�.
	//nCmp           : 0    �񱳱�[0]
	//               : 1    �񱳱�[1]
	[DllImport("pmiMApi.dll")]
    public static extern int pmiCmpGetPos(int nCon, int nCmp, ref int npNum, ref double dpPos);

	//========================================================================================================
	//                                  Master/Slave Motion Control
	//========================================================================================================

	//���� ���� ���������� �����Ѵ�.
	//int nMAxisNo   :   �������� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncSetMaster(int nCon, int nMAxis);

    //���� ���� ���������� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiSyncGetMaster(int nCon, ref int npMAxis);

	//���� ���� ���� ����� ���� ���� �˶� �߻� ������ �����Ѵ�.
	//int nSAxis   : �����̺��� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
	//nAction    : 0 emNOTUSED    - ���� ���� �˶� �߻����� ����
	//             1 emUSED      -  ���� ���� �˶� �߻�
	[DllImport("pmiMApi.dll")]
	public static extern int pmiSyncSetAction(int nCon, int nSAxis, int nAction);

	//���� ���� ���� ����� ���� ���� �˶��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncGetAction(int nCon, int nSAxis, ref int npAction);

	//���� ���� ���� ����� ���� ���� �˶� �߻� ������ �����Ѵ�.
	//int nSAxis   : �����̺��� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
	//dLimit     : 1 ~ 134217727   - ������ ��� �����̺� ������ ���� ���� ��뷮
	[DllImport("pmiMApi.dll")]
	public static extern int pmiSyncSetPosErrorLimit(int nCon, int nSAxis, double dLimit);

	//���� ���� ���� ����� ���� ���� �˶��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncGetPosErrorLimit(int nCon, int nSAxis, ref double dpLimit);

    //���� ���� ���� ���� ��, �ִ� �������� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncGetPosError(int nCon, int nSAxis, ref double dpError, ref double dpMaxError);

	//Position Trigger ��� ��ȣ Ʈ���Ÿ� �����Ѵ�.
	//int nSAxis   : �����̺��� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncBegin(int nCon, int nSAxis);

	//���� �࿡ ���Ͽ� ������ ��� ���⸦ ��ü ��Ų��.
	//int nSAxis   : �����̺��� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
	[DllImport("pmiMApi.dll")]
    public static extern int pmiSyncEnd(int nCon, int nSAxis);

	//========================================================================================================
	//                                  Gantry Motion Control
	//========================================================================================================

    //��Ʈ��(Gantry) ���� ���������� �����Ѵ�.
    //int nMAxisNo   :   �������� ��ȣ( 0 ~ [�ִ� �ళ�� - 1] )
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGantSetMaster(int nCon, int nId, int nMAxis);

    //��Ʈ��(Gantry) ���� ���������� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGantGetMaster(int nCon, int nId, ref int npMAxis);

	//��Ʈ��(Gantry) ���� ���� ����� ���� ���� �˶��� �����Ѵ�.
	//int nSAxis   : �����̺��� ��ȣ( 1,3,5,7 )
	//nAction    : 0  emNOTUSED   ���� ���� �˶� �߻����� ����
	//             1  emUSED      ���� ���� �˶� �߻�
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantSetAction(int nCon, int nId, int nSAxis, int nAction);

	//��Ʈ��(Gantry) ���� ���� ����� ���� ���� �˶��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantGetAction(int nCon, int nId, int nSAxis, ref int npAction);

	//��Ʈ��(Gantry) ���� ������ ��� �����̺� �� ������ ���� ������ ��뷮�� �����Ѵ�.
	//int nSAxis   : �����̺��� ��ȣ( 1,3,5,7 )
	//dLimit     : 1 ~ 134217727   - ������ ��� �����̺� ������ ���� ���� ��뷮
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantSetPosErrorLimit(int nCon, int nId, int nSAxis, double dLimit);

	//��Ʈ��(Gantry) ���� ������ ��� �����̺� �� ������ ���� ���� ��뷮 ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantGetPosErrorLimit(int nCon, int nId, int nSAxis, ref double dpLimit);

    //��Ʈ��(Gantry) ���� ���� ���� ��, �ִ� �������� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGantGetPosError(int nCon, int nId, int nSAxis, ref double dpError, ref double dpMaxError);

	//��Ʈ��(Gantry) ���� ������ ��� �����̺� ���� ���� ��Ų��.
	//int nSAxis   : �����̺��� ��ȣ( 1,3,5,7 )
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantBegin(int nCon, int nId, int nSAxis);

	//��Ʈ��(Gantry) ���� ������ ��� �����̺� �� ������ ���� ��Ų��.
	//int nSAxis   : �����̺��� ��ȣ( 1,3,5,7 )
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGantEnd(int nCon, int nId, int nSAxis);

	//========================================================================================================
	//                                   Manual Pulsar public static extern intS
	//========================================================================================================

    //PA/PB(MPG) input signal mode
    //0x00 1X A/B
    //0x01 2X A/B
    //0x02 4X A/B
    //0x03 CW/CCW(Default)
    [DllImport("pmiMApi.dll")]
    public static extern int pmiMpgSetInType(int nCon, int nAxis, int nDir);

    //PA/PB(MPG) input signal mode
    [DllImport("pmiMApi.dll")]
    public static extern int pmiMpgGetInType(int nCon, int nAxis, ref int npDir);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� �Է� ������ �����Ѵ�.
	//nDir        : 0   emNORMAL    ������
	//            : 1   emRESERVE   ������
	[DllImport("pmiMApi.dll")]
	public static extern int pmiMpgSetDir(int nCon, int nAxis, int nDir);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� �Է� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiMpgGetDir(int nCon, int nAxis, ref int npDir);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� ���� �����Ѵ�.
	//nMultiFactor        :  1 ~ 32      1�� ����޽��� 1 ~ 32 ����� �޽��� �� ����
	//nDivFactor          :  1 ~ 2048    2�� ����޽��� (nDivFactor/2048)�� �������� ���� ����޽� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMpgSetGain(int nCon, int nAxis, int nMultiFactor, int nDivFactor);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� ���� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiMpgGetGain(int nCon, int nAxis, ref int npMultiFactor, ref int npDivFactor);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� �Է� �۾��� �����Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMpgBegin(int nCon, int nAxis);

	//���� �࿡�� MPG(Manual Pulsar)  �޽� �Է� �۾��� ��ü�Ѵ�..
	[DllImport("pmiMApi.dll")]
    public static extern int pmiMpgEnd(int nCon, int nAxis);

	//========================================================================================================
	//                                   Position LATCH public static extern intS
	//========================================================================================================

	//���� ���� Latch ī���� ��ȣ Active Level �����Ѵ�.
	//nLevel   :  0   emLOGIC_A     A����(NORMAL OPEN)
	//         :  1   emLOGIC_B     B����(NORMAL CLOSE)
	[DllImport("pmiMApi.dll")]
    public static extern int pmiLtcSetLevel(int nCon, int nAxis, int nLevel);

	//���� ���� Latch ī���� ��ȣ Active Level ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiLtcGetLevel(int nCon, int nAxis, ref int npLevel);

    //Counter = 0 , Command counter
    //Counter = 1 , Feedback counter
    //Counter = 2 , Error Counter 
    //Counter = 3 , General Counter
    //Counter = 4 , Command Speed
	[DllImport("pmiMApi.dll")]
    public static extern int pmiLtcGetPos(int nCon, int nAxis, int nCount, ref double dpPos);

    //���� ���� Latch ī���� Ȱ��ȭ�� �����Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiLtcSetEnable(int nCon, int nAxis, int nEnable);

	//���� ���� Latch ī���� Ȱ��ȭ�� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiLtcGetEnable(int nCon, int nAxis, ref int npEnable);

    //========================================================================================================
	//                                  Interrupt public static extern intS
	//========================================================================================================
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntSetHandler(int nCon, int nType, uint hWnd, ref int hHandler, uint nMsg);
    
    //
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntSetHandlerEnable(int nCon, int nEnable);

    //-------- Axis--------------------------------------------------------------------------------------
    //
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntSetAxisEnable(int nCon, int nAxis, uint nMask);

	//
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntGetAxisEnable(int nCon, int nAxis, ref uint npMask);    
    
    //BIT0	; �ڵ� ������
    //BIT1	; ���� ���� ��� START ��
    //BIT2	; ���ۿ� 2nd pre register ���� ���� ��
    //BIT3	; Comparator 5�� 2nd pre register ���� ���� ��
    //BIT4	; ���� ���� ��
    //BIT5	; ���� ���� ��
    //BIT6	; ���� ���� ��
    //BIT7	; ���� ���� ��
    //BIT8	; Compatator1 ���� ���� ��
    //BIT9	; Compatator2 ���� ���� ��
    //BIT10	; Compatator3 ���� ���� ��
    //BIT11	; Compatator4 ���� ���� ��
    //BIT12	; Compatator6 ���� ���� ��
    //BIT13	; CLR ��ȣ �Է¿� ���� COUNTER �� RESET ��
    //BIT14	; LTC ��ȣ �Է¿� ���� COUNTER �� LATCH ��
    //BIT15	; ORG ��ȣ �Է¿� ���� COUNTER �� LATCH ��
    //BIT16	; SD �Է� ON ��
    //BIT17	; +DR �Է� ��ȭ ��
    //BIT18	; -DR �Է� ��ȭ ��
    //BIT19	; /STA �Է� ON ��

    //���� ���� ���¸� ��ȯ�մϴ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntGetAxisStatus(int nCon, int nAxis, ref uint npStatus);

    //BIT0 STOP_BY_SLP:   1;	���� ����Ʈ ����Ʈ�� ���� ����
    //BIT1 STOP_BY_SLN:   1;	���� ����Ʈ ����Ʈ�� ���� ����
    //BIT2 STOP_BY_CMP3:  1;	�񱳱�3�� ���� ����
    //BIT3 STOP_BY_CMP4:  1;	�񱳱�4�� ���� ����
    //BIT4 STOP_BY_CMP5:  1;	�񱳱�5�� ���� ����
    //BIT5 STOP_BY_ELP:   1;	+EL �� ���� ����
    //BIT6 STOP_BY_ELN:   1;	-EL �� ���� ����
    //BIT7 STOP_BY_ALM:   1;	�˶��� ���� ����
    //BIT8 STOP_BY_STP:   1;	CSTP�� ���� ����
    //BIT9 STOP_BY_EMG:   1;	EMG�� ���� ����
    //BIT10 STOP_BY_SD:   1;	SD �Է¿� ���� ����
    //BIT11 STOP_BY_DT:   1;	���� ���� DATA �̻� ���� ����
    //BIT12 STOP_BY_IP:   1;	���� ���� �߿� Ÿ ���� �̻� ������ ���� ���� ����
    //BIT13 STOP_BY_PO:   1;	PA/PB �Է¿� buffer counter dml overflow �� ���� ����
    //BIT14 STOP_BY_AO:   1;	���� ���� ���� ��ġ ������ ����� ����
    //BIT15	STOP_BY_EE:   1;	EA/EB �Է� ���� �߻� (���� ���� ����)
    //BIT16	STOP_BY_PE:   1;	PA/PB �Է� ���� �߻� (���� ���� ����)

    //���� ���� Error ���¸� ��ȯ�մϴ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntGetAxisErrStatus(int nCon, int nAxis, ref uint npStatus);

    //-------- DI----------------------------------------------------------------------------------------

    //0 nId => 0  ~ 31
    //1 nId => 32 ~ 63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntSetDiEnable(int nCon, int nId, uint nMask);

    //0 nId => 0  ~ 31
    //1 nId => 32 ~ 63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiIntGetDiStatus(int nCon, int nId, ref uint npStatus);


	//========================================================================================================
	//                                   Digital In/Out public static extern intS
	//========================================================================================================

	//������ �Է� ����̽��κ��� 32�� ä���� ���¸� 32��Ʈ������ ��ȯ�Ѵ�.
	//nId           : 0   - CH0 ~ CH31
	//              : 1   - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiDiGetData(int nCon, int nId, ref uint npData);
                                            
	//������ �Է��� �� ä�δ� ON/OFF ���¸� ��ȯ�Ѵ�.
	//nBit         : 0  ~ 31  - CH0 ~ CH31
	//             : 32 ~ 63  - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiDiGetBit(int nCon, int nBit, ref int npData);
                                            
	//������ ��� ����̽��κ��� 32�� ä���� ���¸� 32��Ʈ������ �����Ѵ�.
	//nId           : 0   - CH0  ~ CH31
	//              : 1   - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
	public static extern int pmiDoSetData(int nCon, int nId, uint nData);
                                            
	//������ ��� ����̽��κ��� 32�� ä���� ���¸� 32��Ʈ������ ��ȯ�Ѵ�.
	//nId           : 0   - CH0 ~ CH31
	//              : 1   - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiDoGetData(int nCon, int nId, ref uint npData);
                                            
                                            
	//������ ����� �� ä�δ� ON/OFF ���¸� �����Ѵ�.
	//nBit         : 0  ~ 31  - CH0 ~ CH31
	//             : 32 ~ 63  - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
	public static extern int pmiDoSetBit(int nCon, int nBit, int nData);
                                            
	//������ ����� �� ä�δ� ON/OFF ���¸� ��ȯ�Ѵ�.
	//nBit         : 0  ~ 31  - CH0  ~ CH31
	//             : 32 ~ 63  - CH32 ~ CH63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiDoGetBit(int nCon, int nBit, ref int npData);
                                            
	//������ �Է��� �� �׷�� ���� ��������� �����Ѵ�.
	//nId           :  0    - CH0  ~ CH15
	//              :  1    - CH16 ~ CH31
	//              :  2    - CH32 ~ CH47
	//              :  3    - CH48 ~ CH63
	[DllImport("pmiMApi.dll")]
    public static extern int pmiDiSetFilterEnable(int nCon, int nId, int nEnable);
                                            
	//������ �Է��� �� �׷�� ���� ��������� ��ȯ�Ѵ�.
	//nId           :  0    - CH0  ~ CH15
	//              :  1    - CH16 ~ CH31
	//              :  2    - CH32 ~ CH47
	//              :  3    - CH48 ~ CH63
	[DllImport("pmiMApi.dll")]
	public static extern int pmiDiGetFilterEnable(int nCon, int nId, ref int npEnable);
                                            
	//������ �Է��� �� �׷�� ���� �ð��� �����Ѵ�.
	//nId           :  0    - CH0  ~ CH15
	//              :  1    - CH16 ~ CH31
	//              :  2    - CH32 ~ CH47
	//              :  3    - CH48 ~ CH63
	//nTime         :  0x00 ~ 0x0F  - ���� �ð�
	[DllImport("pmiMApi.dll")]
	public static extern int pmiDiSetFilterTime(int nCon, int nId, int nTime);
                                            
	//������ �Է��� �� �׷�� ���� �ð��� ��ȯ�Ѵ�.
	//nId           :  0    - CH0 ~ CH15
	//              :  1    - CH16 ~ CH31
	//              :  2    - CH32 ~ CH47
	//              :  3    - CH48 ~ CH63
	[DllImport("pmiMApi.dll")]
	public static extern int pmiDiGetFilterTime(int nCon, int nId, ref int npTime);
                                            
	//========================================================================================================
	//                                   Motion Correction Control
	//========================================================================================================

	//�鷡��(Backlash) �Ǵ� ����(Slip)�� ���� ������ �����Ѵ�.
	//nCorrMode        : emCORR_DIS  0    ���� ����� ��Ȱ��
	//                 : emCORR_BACK 1    ������带 �鷡�� ���� ���
	//                 : emCORR_SLIP 2    ������带 ���� ���� ���
	//dCorrPos         : 0 ~ 4095            ���� �޽��� ��
	//dCorrVel         : 0 ~ 6553500         �����޽��� ��� ���ļ�
	//nCtrMask         : BIT0    1           - �����޽� ��½ÿ� ������ġ Counter�� ����
	//                 : BIT1    1           - �����޽� ��½ÿ� �ǵ����ġ Counter�� ����
	//                 : BIT2    1           - �����޽� ��½ÿ� ���� Counter�� ����
	//                 : BIT3    1           - �����޽� ��½ÿ� ���� Counter�� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetBacklashComps(int nCon, int nAxis, int nCorrMode, double dCorrPos, double dCorrVel, int nCtrMask);

	//�鷡��(Backlash) �Ǵ� ����(Slip)�� ���� ���� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetBacklashComps(int nCon, int nAxis, ref int npCorrMode, ref double dpCorrPos, ref double dpCorrVel, ref int npCtrMask);

	//���� �Ϸ� ���Ŀ� ���� ��� �����Ѵ�.
	//dRevTime     :  0 ~ 65535   ���� ���� �ð� ����
	//dForTime     :  0 ~ 65535   ���� ���� �ð� ����
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSuppressVibration(int nCon, int nAxis, double dRevTime, double dForTime);

	//���� �Ϸ� ���Ŀ� ���� ��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSuppressVibration(int nCon, int nAxis, ref double dpRevTime, ref double dpForTime);

    //====================== DEBUG-LOGGING FUNCTIONS ==============================================//
    //Log ������ ���� �մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogSetFile(string szFilename);

    //Log ������ �о� �ɴϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogGetFile(ref string szFilename);

    //Log ���� Level�� �����մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogSetLevel(int nLevel);

    //Log ���� Level�� ��ȯ�մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogGetLevel(ref int npLevel);

    //Log ���� Ȱ��ȭ�� �����մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogSetEnable(int nEnable);

    //Log ���� Ȱ��ȭ�� ��ȯ�մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiDLogGetEnable(ref int npEnable);

    //Error �ڵ带 ��ȯ�մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiErrGetCode(int nCon, ref int npCode);

    //Error �ڵ��� ������ ��ȯ�մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiErrGetString(int nCon, int nCode, ref string szpStr);

    //���� ���� Error �ڵ带 ��ȯ�մϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiErrAxGetCode(int nCon, int nAxis, ref int npCode);

	//========================================================================================================
	//                                   GENERAL public static extern intS
	//========================================================================================================

    //�ùķ��̼� ��� Ȱ��ȭ ���θ� �����Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxSetSimulEnable(int nCon, int nAxis, int nEnable);

    //�ùķ��̼� ��� Ȱ��ȭ ���θ� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiAxGetSimulEnable(int nCon, int nAxis, ref int npEnable);

    //+el.-el.sd.org. alm.inp
    //0 <= nTime <= 15
    /*���� �Է� ��ȣ�� ����	��ȣ ���� �ð�
	CLK=16MHz����	���� �Է� ��ȣ�� ����	��ȣ ���� �ð���CLK=16MHz����
	PALF3	PALF2	PALF1	PALF0				PALF3	PALF2	PALF1	PALF0	
	Low		Low		Low		Low		1 ��sec	    Hi		Low		Low		Low		0.256 msec
	Low		Low		Low		Hi		2 ��sec	    Hi		Low		Low		Hi		0.512 msec
	Low		Low		Hi		Low		4 ��sec		Hi		Low		Hi		Low		1.02 msec(default)
	Low		Low		Hi		Hi		8 ��sec		Hi		Low		Hi		Hi		2.05 msec
	Low		Hi		Low		Low		16 ��sec	Hi		Hi		Low		Low		4.10 msec
	Low		Hi		Low		Hi		32 ��sec	Hi		Hi		Low		Hi		8.19 msec
	Low		Hi		Hi		Low		64 ��sec	Hi		Hi		Hi		Low		16.4 msec
	Low		Hi		Hi		Hi		128 ��sec	Hi		Hi		Hi		Hi		32.8 msec
    */
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGnSetFilterTime(int nCon, int nTime);
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGnGetFilterTime(int nCon, ref int npTime);	

	//��� ���� ��ȣ Active ������ �����Ѵ�..
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnSetEmgLevel(int nCon, int nLevel);
                                           
	//��� ���� ��ȣ Active ������ ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnGetEmgLevel(int nCon, ref int npLevel);

	//��� ��� ������ ���� ���� ��� �����ϴ� �Լ��̴�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnSetEStop(int nCon);
                                            
	//����� ����� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnGetAxesNum(int nCon, ref int npNAxesNum);
                                            
	//������ ����� ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnGetDioNum(int nCon, ref int npNDiChNum, ref int npNDoChNum);
                                            
	//�ǵ�� �ӵ� Ȯ�� �� �ֱ� �����Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnSetCheckActVel(int nEnable, int nInterval);

	//�ǵ�� �ӵ� Ȯ�� �� �ֱ� ��ȯ�Ѵ�..
	[DllImport("pmiMApi.dll")]
	public static extern int pmiGnGetCheckActVel(ref int npEnable, ref int npInterval);

	//�ش� Controller�� ���̽� �𵨸��� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiConGetModel( int nCon, ref int npModel);

    //Ư�� Controller�� ���̽� �𵨸��� ��ȯ�Ѵ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiConGetModelEx(int nCon, ref int npModel);

    //�ش� Controller�� Firmware ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiConGetFwVersion(int nCon, ref int npVer);
                                            
	//�ش� Controller�� Hardware ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiConGetHwVersion(int nCon, ref int npVer);
                                            
	//MAPI ������ ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
	public static extern int pmiConGetMApiVersion(int nCon, ref int npVer);

    //������ LED ON/OFF �մϴ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiConSetCheckOn(int nCon, int nOn);
                                           
	//������ LED ���¸� ��ȯ�Ѵ�.
	[DllImport("pmiMApi.dll")]
    public static extern int pmiConGetCheckOn(int nCon, ref int npOn);

    //�������� ���¸� �о� �ɴϴ�.
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGnGetRegister(int nCon, int nAxis, int nRNo, ref int npValue);

    //
	[DllImport("pmiMApi.dll")]
    public static extern int pmiGnGetString(int nCon, int nAxis, int nStrID, ref string szpBuffer);

    //���� ���� H/W 
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGnSetStaSignal(int nCon, int nEnable);

    //���� ���� H/W 
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGnGetStaSignal(int nCon, ref int npEnable);

    //���� ���� H/W
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGnSetStpSignal(int nCon, int nEnable);

    //���� ���� H/W
    [DllImport("pmiMApi.dll")]
    public static extern int pmiGnGetStpSignal(int nCon, ref int npEnable);

    //========================================================================================================
    //                                   Motion-NET FUNCTIONS
    //========================================================================================================

    //��� �ʱ�ȭ
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetSysComm(int nCon);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetReset(int nCon);

    //����Ŭ�� ���
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetCyclicBegin(int nCon);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetCyclicEnd(int nCon);

    //��� �ӵ�
    // 0 : 2.5 Mbps
    // 1 : 5 Mbps
    // 2 : 10 Mbps
    // 3 : 20 Mbps
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetSetCommSpeed(int nCon, int nCommSpeed);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetCommSpeed(int nCon, ref int npCommSpeed);

    //��� Digital In/Out FUNCTIONS
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDiGetData(int nCon, int nStNo, ref uint npData);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDiGetBit(int nCon, int nStNo, int nBit, ref uint npData);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDoSetData(int nCon, int nStNo, uint nData);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDoGetData(int nCon, int nStNo, ref uint npData);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDoSetBit(int nCon, int nStNo, int nBit, uint nData);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetDoGetBit(int nCon, int nStNo, int nBit, ref uint npData);

    //��� ���� ����
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetCommErrNum(int nCon, ref int npErrNum);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetCommErrNumClear(int nCon);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetCyclicErrFlag(int nCon, int nStNo, ref int npFlag);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetCyclicErrFlagClear(int nCon, int nStNo);

    // ����Ŭ�� ��� ���� ����
    // 1 || 0  ��Ʈ
    // 0    0  ��� ����
    // 0    1  ����Ŭ�� ��� ��
    // 1    0  ����Ŭ�� ��� ����. ���� ���� ���� ����
    // 1    1  ����Ŭ�� ��� �� ���� �߻�
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetCyclicStatus(int nCon, ref uint npStatus);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetCyclicSpeed(int nCon, ref int npTime);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetSlaveTotal(int nCon, ref int npSlvNum);

    //����̽� ����
    // 2 || 1 || 0  ��Ʈ
    // 0    0    0  : 32�� ��� ����
    // 0    1    0  : 16�� ��� 16�� �Է� 
    // 1    1    1  : 32�� �Է� ����
    // 3            ��Ʈ
    // 0            : I/O ����
    // 1            : Motion ����
    // 4            ��Ʈ
    // 0            : ��� ����
    // 1            : ���
    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetSlaveInfo(int nCon, int nStNo, ref uint npStatus);

    [DllImport("pmiMApi.dll")]
    public static extern int pmiNetGetSlaveDioNum(int nCon, int nStNo, ref int npDiNum, ref int npDoNum);
}