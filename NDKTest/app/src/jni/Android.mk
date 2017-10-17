LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
LOCAL_MODULE := test-ndk
LOCAL_SRC_FILES := E:/WorkspaceMinh/MagicDrawing/NDKTest/app/src/cpp/NDKTest.cpp
# include $(BUILD_STATIC_LIBRARY)
# include $(BUILD_EXECUTABLE)
include $(BUILD_SHARED_LIBRARY)