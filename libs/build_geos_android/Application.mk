#ANDROID APPLICATION MAKEFILE

# 8 variables use on total 15 variable on Application.mk
APP_BUILD_SCRIPT := $(call my-dir)/Android.mk
APP_PROJECT_PATH := $(call my-dir)
APP_OPTIM := release
APP_PLATFORM 	:= android-14
APP_STL 		:= gnustl_static
APP_CPPFLAGS 	:= -fexceptions -frtti
APP_ABI 		:= x86_64
APP_MODULES     := geos
# APP_PROJECT_PATH
# APP_MODULES
# APP_OPTIM
# APP_CFLAGS
# APP_CPPFLAGS
# APP_LDFLAGS
# APP_BUILD_SCRIPT
# APP_ABI
# APP_ABI := armeabi armeabi-v7a x86 mips
# APP_PLATFORM
# APP_STL
# APP_SHORT_COMMANDS
# NDK_TOOLCHAIN_VERSION
# APP_PIE
# APP_THIN_ARCHIVE