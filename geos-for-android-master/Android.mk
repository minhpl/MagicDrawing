LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
LOCAL_MODULE := tang
LOCAL_C_INCLUDES := ../lib/geos-3.6.2/include
LOCAL_SHARED_LIBRARIES := libs/$(TARGET_ARCH_ABI)/libgeos.so
