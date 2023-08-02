@echo off

:: 用户配置区域开始
:: 环境名称
set ENV_NAME=ml-agents
:: 默认训练ID
set DEFAULT_ID=default_run
:: 用户配置区域结束

:: 获取批处理文件的目录
set TRAIN_DIR=%~dp0

:: 激活环境
echo 激活%ENV_NAME%环境...
call activate %ENV_NAME%

:: 进入训练目录
echo 进入项目训练目录%TRAIN_DIR%...
cd /d %TRAIN_DIR%

:enter_id
:: 接受用户输入训练ID名称
set /p train_id=请输入训练ID名称（直接按回车使用默认ID）:
if "%train_id%"=="" set train_id=%DEFAULT_ID%

:: 检查训练ID是否存在
if exist .\results\%train_id%\*.* goto id_exists
goto ask_tensorboard

:id_exists
echo 训练ID已存在。
echo 是否继续训练？(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" goto ask_tensorboard
if /i "%continue_choice%"=="N" goto enter_id
goto id_exists

:ask_tensorboard
echo 是否开启可视化界面？(Y/N)
set /p tensorboard_choice=
if /i "%tensorboard_choice%"=="Y" goto start_tensorboard
if /i "%tensorboard_choice%"=="N" goto start_training_based_on_choice
goto ask_tensorboard

:start_tensorboard
:: 开启 TensorBoard
start /min cmd /C tensorboard --logdir results
timeout /t 3 /nobreak >NUL
start http://localhost:6006

:start_training_based_on_choice
if exist .\results\%train_id%\*.* goto start_training
goto start_new_training

:start_new_training
echo 开始新训练...
echo mlagents-learn config.yaml --run-id=%train_id%
mlagents-learn config.yaml --run-id=%train_id%
goto end

:start_training
echo 继续训练...
echo mlagents-learn config.yaml --run-id=%train_id% --resume
mlagents-learn config.yaml --run-id=%train_id% --resume
goto end

:end
echo 训练完成！
pause
