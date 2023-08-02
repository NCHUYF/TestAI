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

goto start_new_training

:id_exists
:confirm_continue
echo ѵ��ID�Ѵ��ڡ�
echo �Ƿ����ѵ����(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" goto start_training
if /i "%continue_choice%"=="N" goto enter_id
goto confirm_continue

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
