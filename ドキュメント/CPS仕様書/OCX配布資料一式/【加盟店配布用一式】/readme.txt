2019.10.11 �\���}�`�Z��

�P�D�C���X�g�[���[�ɂ���

	�y���p���@�z
	�C���X�g�[���[�uOCX_Installer.bat�v��"�Ǘ��Ҍ���"�Ŏ��s���Ă��������B
	���L�̏������s���OCX�����p�\�ɂȂ�܂��B

	����L���@�ŊǗ��Ҍ����Ŏ��s�ł��Ȃ��ꍇ�́A���L�̕��@�Ŏ��{���������B
	�@�P�D�R�}���h�v�����v�g���Ǘ��Ҍ����Ŏ��s
	�@�@�@�uWin�{s�v�Ō����o�[���o��̂ŁA"cmd"�Ɠ���
�@�@�@�@�@�@�@�R�}���h�v�����v�g���o�Ă���Ǝv���̂ŉE�N���b�N���Ǘ��҂Ƃ��Ď��s

	�@�Q�D�R�}���h�v�����v�g�ŃC���X�g�[���̃f�B���N�g���Ɉړ�
	   �@�@"cd (�C���X�g�[���[�̃p�X)"�@�Ɠ��͂���ƈړ��ł��܂��B

	�@�R�D�R�}���h�v�����v�g��"OCX_Installer.bat"�Ɠ��͂���Enter

	
		�E�uC:\OPOS\CPS\EVRW\CPS_PaymentModule�v�f�B���N�g���̍쐬
		�EOCX����L�f�B���N�g���ɃR�s�[
		�ECCO�̃C���X�g�[��
		�EVisualStudio2008 runtime�̓o�^
		�E���W�X�g���̓o�^
		�EWindows�ւ�OCX�̓o�^
		
	�y���O�ݒ�z
		�@���t�@�C���̔z�u
		�@���L�̃f�B���N�g���Ɍ��t�@�C����z�u���Ă��������B
		�@�uC:\OPOS\CPS\EVRW\CPS_PaymentModule�v
			rsa_private_key.pem
			rsa_public_key.pem
			serverPublicKey.pem

		�ASettings.json�Ɏ��O�ݒ�����L�ڂ��Ă��������B
		�@�ݒ���e�́uEVRW_APG_PAYTREECPMSS.docx�v��"2.3	�ݒ�t�@�C���\��"���Q�Ƃ��������B
		�@�X�^�u��ISS���y�і{�Ԋ��ւ̐ڑ���͖{�t�@�C���ɋL�ڂ��܂��B
		�@�{�t�@�C����Open���\�b�h���s���ɓǂݍ��܂�邽�߁A�L�ړ��e��ύX�����ꍇ�͍ēxClose��Open�����ēǂݍ��݂Ȃ����Ă��������B

	�y���ӎ����z
		�E�uC:\OPOS\CPS\EVRW\CPS_PaymentModule�v�ȉ��̃t�@�C���͈ړ������Ȃ��ł��������B

�Q�D�e�X�g�c�[���ɂ���
	OCX�̓�����m�F����e�X�g�c�[���uEVRWTest.exe�v�𓯍����Ă��܂��B
	OCX���C���X�g�[������Ă��Ȃ��ꍇ�N�����Ȃ����߂����Ӊ������B
	
	�y�g�p���@�z
	�E�uEVRWTest.exe�v��"�Ǘ��Ҍ���"�ŋN�����܂��B
	�E���s�������{�^�����N���b�N���邱�ƂŔC�ӂ�OCX�̃��\�b�h�����s�\�ł��B
	�E�v���p�e�B�����l�Ƀ{�^�����N���b�N���邱�ƂŒl��ݒ�\�ł��B
	�EAsyncMode�v���p�e�B��TRUE�̏ꍇ�|�b�v�A�b�v��OCX�̃C�x���g���ʒm����܂��B
	
	�y�����z
	�E�x���̎��s
		�P�DOpen("CPS_PaymentModule");
		�Q�DClaimDevice(5000);
		�R�DDeviceEnabled�v���p�e�B��TRUE�ɐݒ�
		�S�DAsyncMode�v���p�e�B��TRUE�ɐݒ�
		�T�DSetParameterInfomation()���\�b�h�𗘗p���Č��ςɕK�v�ȏ����^�O�ݒ�
			�uPAYTREE�Ή�OPOS�h���C�o�ݒ荀�ڈꗗ.xlsm�v���Q��
			�x���̏ꍇ�͉��L�̃^�O��ݒ肭�������B
			
				�[�����ʔԍ�	terminalUniqueCode
				�V���A��No.	serialNo
				�x���`�����l��	payType
				���D���z	amount
				���V�[�g�ԍ�	receiptNo
				�o�[�R�[�h	oneTimeCode

				�X�^�u�@�@�@�@�@stubUrl�FstubPay/version=1.0/B01/0/0/

		�U�D"���l",�y��"�g�����"����͂���ꍇ��AdditionalSecurityInfomation�v���p�e�B��JSON�`���œ���
		�V�DSubtractValue()�ɂ��x��API�����s
		�W�D���s���ʂ�OutpuCompleteEvent/ErrorEvent�̂ǂ��炩�Œʒm�����
			�W�|�P�D�������@OutpuCompleteEvent���ʒm�����
				�T�[�o�[����̃��X�|���X��AdditionalSecurityInfomation�v���p�e�B�y�у^�O�ɐݒ肳���B
			
			�W�|�Q�D���s���@ErrorEvent���ʒm�����
				�T�[�o�[����̃��X�|���X��AdditionalSecurityInfomation�ɐݒ肳���B


�R�D��������
�@�E���݃��O�@�\�ɕs��������A���t���ς��ƃC���N�������g�����@�\���L���ɂȂ��Ă��炸�B
�@�@���O���㏑������铮��ƂȂ��Ă���܂��B������Ɋւ��Ă͏C���������܂��̂ł��������������B
