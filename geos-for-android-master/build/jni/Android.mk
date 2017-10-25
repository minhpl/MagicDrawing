# ./configure --build=x86_64-pc-linux-gnu --host=arm-linux-eabi
# find $(GEOS_PATH)/ -name "*.cpp" | grep -Ev "tests|doc" | sort | awk '{ print "\t"$1" \\" }'
GEOS_PATH := E:/WorkspaceMinh/MagicDrawing/libs/geos-3.6.2
GEOS_SOURCE:= E:/WorkspaceMinh/MagicDrawing/GEOS


# LOCAL_MODULE := tang
# LOCAL_SHORT_COMMANDS := true
# APP_SHORT_COMMANDS := true
# LOCAL_C_INCLUDES := \
# 		$(GEOS_PATH)/src \
#     $(GEOS_PATH)/include
	
# LOCAL_SRC_FILES := $(GEOS_SOURCE)/Source.cpp
# LOCAL_SHARED_LIBRARIES = $(TARGET_ARCH_ABI)

# include $(BUILD_SHARED_LIBRARY)




LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
LOCAL_MODULE := geos
LOCAL_SRC_FILES :=  $(LOCAL_PATH)/../../libs/$(TARGET_ARCH_ABI)/libgeos.so
LOCAL_EXPORT_C_INCLUDES := $(GEOS_PATH)/include
include $(PREBUILT_SHARED_LIBRARY)


include $(CLEAR_VARS)
LOCAL_MODULE    := tangram
LOCAL_SRC_FILES := $(GEOS_SOURCE)/Source.cpp
LOCAL_SHARED_LIBRARIES := geos
include $(BUILD_SHARED_LIBRARY)