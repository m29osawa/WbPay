
keytool���s���̃p�����[�^�[

PassWord�i�L�[�X�g�A�̃p�X���[�h�j=Wellba!1122


First and Last Name�i�����j=wba.jp
Organizational Unit�i�g�D�P�ʁj=MockServer
Organization�i�g�D�j=Wellba
City/Locality�i�s�s�^�n���j=Azabu
State/Province�i�s���{���^�B�j=Tokyo
Country Code�i���R�[�h�j=JP


���̐���
keytool -genkey -keystore .keystore -alias tomcat -keyalg RSA

Current dir ��.keystore���ł���B"%USERPROFILE%"\.keystore����
�������ꂽ����90���Ԃ����L���ł͂Ȃ��炵���F
90���ԗL����2,048�r�b�g��RSA�̃L�[�E�y�A�Ǝ��ȏ����^�ؖ���(SHA256withRSA)�𐶐����Ă��܂�

CSR�t�@�C���̍쐬
keytool -certreq -keyalg RSA -alias tomcat -file tomcat.csr

�ؖ���������ꍇ�́A����csr�t�@�C����F�؋ǂɑ���B

���̌�A���肵���ؖ�����keystore��import����B

�ؖ����̃G�N�X�|�[�g

keytool -keystore .keystore -exportcert -alias tomcat -file tomcat.cer -rfc

Windows �̏ؖ����̊Ǘ�

���[�J���R���s���[�^�[�ɕۑ�����Ă���A�T�[�o�[�ؖ����̊Ǘ�

certlm.msc

���[�U�[�A�J�E���g���ؖ����郆�[�U�[�ؖ����̊Ǘ�

certmgr.msc


