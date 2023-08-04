@echo off

:: �û���������ʼ
:: ��������
set ENV_NAME=ml-agents
:: Ĭ��ѵ��ID
set DEFAULT_ID=default_run
:: ��ִ�л�����·��
set EXEC_ENV_PATH=../../Output/TestAI.exe

:: �û������������

:: ��ȡ�������ļ���Ŀ¼
set TRAIN_DIR=%~dp0

:: �����
echo ����%ENV_NAME%����...
call activate %ENV_NAME%

:: ����ѵ��Ŀ¼
echo ������Ŀѵ��Ŀ¼%TRAIN_DIR%...
cd /d %TRAIN_DIR%

:: ��ȡ�����ѵ��ID
call :enter_id
call :check_id_exists

:end
echo ѵ����ɣ�
pause
exit /b

:: ==============================================================================
:: ����ģ������ʼ
:: ==============================================================================

:enter_id
:: �����û�����ѵ��ID����
:: ------------------------------------------------------------------------------
set /p train_id=������ѵ��ID���ƣ�ֱ�Ӱ��س�ʹ��Ĭ��ID��:
if "%train_id%"=="" set train_id=%DEFAULT_ID%
exit /b

:check_id_exists
:: ���ѵ��ID�Ƿ����
:: ------------------------------------------------------------------------------
if exist .\results\%train_id%\*.* (
    call :id_exists
) else (
    call :ask_tensorboard
)
exit /b

:id_exists
:: ���ѵ��ID�Ƿ���ڲ�������Ӧ����
:: ------------------------------------------------------------------------------
echo ѵ��ID�Ѵ��ڡ�
echo �Ƿ����ѵ����(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" call :ask_tensorboard
if /i "%continue_choice%"=="N" call :enter_id & call :check_id_exists
exit /b

:ask_tensorboard
:: ѯ���Ƿ������ӻ�����
:: ------------------------------------------------------------------------------
echo �Ƿ������ӻ����棿(Y/N)
set /p tensorboard_choice=
if /i "%tensorboard_choice%"=="Y" call :start_tensorboard
call :ask_initialize
exit /b

:start_tensorboard
:: ���� TensorBoard
:: ------------------------------------------------------------------------------
start /min cmd /C tensorboard --logdir results
timeout /t 3 /nobreak >NUL
start http://localhost:6006
exit /b

:ask_initialize
:: ѯ���Ƿ������ѵ��ID��ʼ��ѵ��
:: ------------------------------------------------------------------------------
echo �Ƿ������ѵ��ID��ʼ��ѵ����(����ID���ƻ���ֱ�ӻس�����)
set "initialize_from_choice="
set /p initialize_from_choice=
if not defined initialize_from_choice (
    call :start_training_based_on_choice
    exit /b
)
if not exist .\results\%initialize_from_choice%\*.* (
    call :initialize_not_found
) else (
    call :initialize_from
)
exit /b

:initialize_not_found
:: �����ѵ��ID�����ڣ�����������
:: ------------------------------------------------------------------------------
echo �����ѵ��ID�����ڣ����������롣
call :ask_initialize
exit /b

:initialize_from
:: ��ʼ��ѵ����ѵ��ID
:: ------------------------------------------------------------------------------
echo ��ʼ��ѵ����ѵ��ID: %initialize_from_choice%
call :start_training_based_on_choice
exit /b

:start_training_based_on_choice
:: ����ѡ��ʼѵ��
:: ------------------------------------------------------------------------------
if exist .\results\%train_id%\*.* (
    call :start_training
) else (
    call :start_new_training
)
exit /b

:start_new_training
:: ��ʼ��ѵ��
:: ------------------------------------------------------------------------------
if "%initialize_from_choice%"=="" (
    echo ��ʼ��ѵ��...
    echo mlagents-learn config.yaml --run-id=%train_id% --env=%EXEC_ENV_PATH%
    mlagents-learn config.yaml --run-id=%train_id% --env=%EXEC_ENV_PATH% 
) else (
    echo ��ʼ��ѵ������ѵ��ID: %initialize_from_choice% ��ʼ��...
    echo mlagents-learn config.yaml --run-id=%train_id% --initialize-from=%initialize_from_choice% --env=%EXEC_ENV_PATH% 
    mlagents-learn config.yaml --run-id=%train_id% --initialize-from=%initialize_from_choice% --env=%EXEC_ENV_PATH% 
)
exit /b

:start_training
:: ����ѵ��
:: ------------------------------------------------------------------------------
echo ����ѵ��...
echo mlagents-learn config.yaml --run-id=%train_id% --resume --env=%EXEC_ENV_PATH%
mlagents-learn config.yaml --run-id=%train_id% --resume --env=%EXEC_ENV_PATH%
exit /b

