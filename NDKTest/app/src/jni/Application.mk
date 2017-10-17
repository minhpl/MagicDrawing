APP_BUILD_SCRIPT := $(call my-dir)/Android.mk
# APP_STL := gnustl_shared
APP_STL := c++_shared
APP_PLATFORM := android-14
APP_ABI := all
APP_CPPFLAGS += -std=c++11 -frtti -fexceptions  # Enable c++11 extentions in source code