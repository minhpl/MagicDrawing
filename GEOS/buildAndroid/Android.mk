# ./configure --build=x86_64-pc-linux-gnu --host=arm-linux-eabi
# find $(GEOS_PATH)/ -name "*.cpp" | grep -Ev "tests|doc" | sort | awk '{ print "\t"$1" \\" }'
GEOS_PATH := E:/WorkspaceMinh/MagicDrawing/libs/geos-3.6.2
GEOS_SOURCE:= E:/WorkspaceMinh/MagicDrawing/GEOS
BOOST_PATH := E:/WorkspaceMinh/MagicDrawing/libs/boost_1_65_1


LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
LOCAL_C_INCLUDES := \
		$(GEOS_PATH)/src \
    $(GEOS_PATH)/include

LOCAL_MODULE := geos
LOCAL_SRC_FILES :=  $(LOCAL_PATH)/../../libs/libs_geos/$(TARGET_ARCH_ABI)/libgeos.a
include $(PREBUILT_STATIC_LIBRARY)


include $(CLEAR_VARS)
LOCAL_C_INCLUDES := \
		$(GEOS_PATH)/src \
    $(GEOS_PATH)/include\
    $(BOOST_PATH)
LOCAL_MODULE    := tangram
LOCAL_SRC_FILES := $(GEOS_SOURCE)/Source.cpp
LOCAL_STATIC_LIBRARIES := geos
include $(BUILD_EXECUTABLE)
# include $(BUILD_SHARED_LIBRARY)