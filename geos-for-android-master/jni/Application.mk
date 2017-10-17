#ANDROID APPLICATION MAKEFILE

# 8 variables use on total 14 variable on Application.mk
APP_BUILD_SCRIPT := $(call my-dir)/Android.mk
APP_PROJECT_PATH := $(call my-dir)

APP_OPTIM := release

APP_PLATFORM 	:= android-14
APP_STL 		:= gnustl_static
APP_CPPFLAGS 	:= -fexceptions -frtti
# APP_ABI 		:= armeabi armeabi-v7a x86_64
APP_ABI 		:= all
APP_MODULES     := geos
