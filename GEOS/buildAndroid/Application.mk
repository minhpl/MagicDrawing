#ANDROID APPLICATION MAKEFILE

# 8 variables use on total 14 variable on Application.mk
APP_BUILD_SCRIPT := $(call my-dir)/Android.mk
APP_PROJECT_PATH := $(call my-dir)

APP_OPTIM := release

APP_PLATFORM 	:= android-16
APP_STL 		:= gnustl_static
APP_CPPFLAGS 	:= -fexceptions -frtti -std=c++11

APP_ABI 		:= x86 armeabi-v7a  
APP_MODULES     := tangram
APP_PIE := true