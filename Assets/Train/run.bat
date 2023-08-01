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

goto start_new_training

:id_exists
:confirm_continue
echo 训练ID已存在。
echo 是否继续训练？(Y/N)
set /p continue_choice=
if /i "%continue_choice%"=="Y" goto start_training
if /i "%continue_choice%"=="N" goto enter_id
goto confirm_continue

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
