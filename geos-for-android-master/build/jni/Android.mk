LOCAL_PATH := $(call my-dir)

LOCAL_PATH := $(call my-dir)
# ./configure --build=x86_64-pc-linux-gnu --host=arm-linux-eabi
# find $(GEOS_PATH)/ -name "*.cpp" | grep -Ev "tests|doc" | sort | awk '{ print "\t"$1" \\" }'
GEOS_PATH := E:/WorkspaceMinh/MagicDrawing/lib/geos-3.6.2
GEOS_SOURCE:= E:/WorkspaceMinh/MagicDrawing/GEOS
LOCAL_MODULE := tang
LOCAL_SHORT_COMMANDS := true
APP_SHORT_COMMANDS := true
LOCAL_C_INCLUDES := \
		$(GEOS_PATH)/src \
    $(GEOS_PATH)/include
	
LOCAL_SRC_FILES := $(GEOS_SOURCE)/Source.cpp


include $(BUILD_SHARED_LIBRARY)
