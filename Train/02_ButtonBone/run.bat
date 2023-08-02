@echo off

:: �û���������ʼ
:: ��������
set ENV_NAME=ml-agents
:: Ĭ��ѵ��ID
set DEFAULT_ID=default_run
:: �û������������

:: ��ȡ�������ļ���Ŀ¼
set TRAIN_DIR=%~dp0

:: �����
echo ����%ENV_NAME%����...
call activate %ENV_NAME%

:: ����ѵ��Ŀ¼
echo ������Ŀѵ��Ŀ¼%TRAIN_DIR%...
cd /d %TRAIN_DIR%

:enter_id
:: �����û�����ѵ��ID����
set /p train_id=������ѵ��ID���ƣ�ֱ�Ӱ��س�ʹ��Ĭ��ID��:
if "%train_id%"=="" set train_id=%DEFAULT_ID%

:: ���ѵ��ID�Ƿ����
if exist .\results\%train_id%\*.* goto id_exists
goto ask_tensorboard

:id_exists
echo ѵ��ID�Ѵ��ڡ�
echo �Ƿ����ѵ����(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" goto ask_tensorboard
if /i "%continue_choice%"=="N" goto enter_id
goto id_exists

:ask_tensorboard
echo �Ƿ������ӻ����棿(Y/N)
set /p tensorboard_choice=
if /i "%tensorboard_choice%"=="Y" goto start_tensorboard
if /i "%tensorboard_choice%"=="N" goto start_training_based_on_choice
goto ask_tensorboard

:start_tensorboard
:: ���� TensorBoard
start /min cmd /C tensorboard --logdir results
timeout /t 3 /nobreak >NUL
start http://localhost:6006

:start_training_based_on_choice
if exist .\results\%train_id%\*.* goto start_training
goto start_new_training

:start_new_training
echo ��ʼ��ѵ��...
echo mlagents-learn config.yaml --run-id=%train_id%
mlagents-learn config.yaml --run-id=%train_id%
goto end

:start_training
echo ����ѵ��...
echo mlagents-learn config.yaml --run-id=%train_id% --resume
mlagents-learn config.yaml --run-id=%train_id% --resume
goto end

:end
echo ѵ����ɣ�
pause
