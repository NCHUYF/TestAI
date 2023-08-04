@echo off

:: 用户配置区域开始
:: 环境名称
set ENV_NAME=ml-agents
:: 默认训练ID
set DEFAULT_ID=default_run
:: 可执行环境的路径
set EXEC_ENV_PATH=../../Output/TestAI.exe

:: 用户配置区域结束

:: 获取批处理文件的目录
set TRAIN_DIR=%~dp0

:: 激活环境
echo 激活%ENV_NAME%环境...
call activate %ENV_NAME%

:: 进入训练目录
echo 进入项目训练目录%TRAIN_DIR%...
cd /d %TRAIN_DIR%

:: 获取并检查训练ID
call :enter_id
call :check_id_exists

:end
echo 训练完成！
pause
exit /b

:: ==============================================================================
:: 方法模块区域开始
:: ==============================================================================

:enter_id
:: 接受用户输入训练ID名称
:: ------------------------------------------------------------------------------
set /p train_id=请输入训练ID名称（直接按回车使用默认ID）:
if "%train_id%"=="" set train_id=%DEFAULT_ID%
exit /b

:check_id_exists
:: 检查训练ID是否存在
:: ------------------------------------------------------------------------------
if exist .\results\%train_id%\*.* (
    call :id_exists
) else (
    call :ask_tensorboard
)
exit /b

:id_exists
:: 检查训练ID是否存在并做出相应操作
:: ------------------------------------------------------------------------------
echo 训练ID已存在。
echo 是否继续训练？(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" call :ask_tensorboard
if /i "%continue_choice%"=="N" call :enter_id & call :check_id_exists
exit /b

:ask_tensorboard
:: 询问是否开启可视化界面
:: ------------------------------------------------------------------------------
echo 是否开启可视化界面？(Y/N)
set /p tensorboard_choice=
if /i "%tensorboard_choice%"=="Y" call :start_tensorboard
call :ask_initialize
exit /b

:start_tensorboard
:: 开启 TensorBoard
:: ------------------------------------------------------------------------------
start /min cmd /C tensorboard --logdir results
timeout /t 3 /nobreak >NUL
start http://localhost:6006
exit /b

:ask_initialize
:: 询问是否从其他训练ID初始化训练
:: ------------------------------------------------------------------------------
echo 是否从其他训练ID初始化训练？(输入ID名称或者直接回车跳过)
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
:: 输入的训练ID不存在，请重新输入
:: ------------------------------------------------------------------------------
echo 输入的训练ID不存在，请重新输入。
call :ask_initialize
exit /b

:initialize_from
:: 初始化训练从训练ID
:: ------------------------------------------------------------------------------
echo 初始化训练从训练ID: %initialize_from_choice%
call :start_training_based_on_choice
exit /b

:start_training_based_on_choice
:: 基于选择开始训练
:: ------------------------------------------------------------------------------
if exist .\results\%train_id%\*.* (
    call :start_training
) else (
    call :start_new_training
)
exit /b

:start_new_training
:: 开始新训练
:: ------------------------------------------------------------------------------
if "%initialize_from_choice%"=="" (
    echo 开始新训练...
    echo mlagents-learn config.yaml --run-id=%train_id% --env=%EXEC_ENV_PATH%
    mlagents-learn config.yaml --run-id=%train_id% --env=%EXEC_ENV_PATH% 
) else (
    echo 开始新训练并从训练ID: %initialize_from_choice% 初始化...
    echo mlagents-learn config.yaml --run-id=%train_id% --initialize-from=%initialize_from_choice% --env=%EXEC_ENV_PATH% 
    mlagents-learn config.yaml --run-id=%train_id% --initialize-from=%initialize_from_choice% --env=%EXEC_ENV_PATH% 
)
exit /b

:start_training
:: 继续训练
:: ------------------------------------------------------------------------------
echo 继续训练...
echo mlagents-learn config.yaml --run-id=%train_id% --resume --env=%EXEC_ENV_PATH%
mlagents-learn config.yaml --run-id=%train_id% --resume --env=%EXEC_ENV_PATH%
exit /b

