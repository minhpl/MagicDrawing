# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

# Creates and names a library, sets it as either STATIC
# or SHARED, and provides the relative paths to its source code.
# You can define multiple libraries, and CMake builds them for you.
# Gradle automatically packages shared libraries with your APK.


set(pathToOpenCV E:\\WorkspaceMinh\\MagicDrawing\\OpenCV-android-sdk\\sdk\\native)
include_directories(${pathToOpenCV}/jni/include)

set(testPath src/main/)
message(STATUS ${testPath})

add_library( # Sets the name of the library.
             RenderingPlugin

             # Sets the library as a shared library.
             SHARED

             # Provides a relative path to your source file(s).
              src/main/cpp/RenderingPlugin.cpp
              src/main/cpp/RenderAPI.cpp
              src/main/cpp/RenderAPI_D3D11.cpp
              src/main/cpp/RenderAPI_D3D12.cpp
              src/main/cpp/RenderAPI_D3D9.cpp
              src/main/cpp/RenderAPI_Metal.mm
              src/main/cpp/RenderAPI_OpenGL2.cpp
              src/main/cpp/RenderAPI_OpenGLCoreES.cpp
             # src/main/cpp/native-lib.cpp
             )

add_library( lib_opencv SHARED IMPORTED )
set_target_properties(lib_opencv PROPERTIES IMPORTED_LOCATION E:\\WorkspaceMinh\\MagicDrawing\\MyApplication5\\app\\src\\main\\jniLibs\\${ANDROID_ABI}/libopencv_java3.so)

# Searches for a specified prebuilt library and stores the path as a
# variable. Because system libraries are included in the search path by
# default, you only need to specify the name of the public NDK library
# you want to add. CMake verifies that the library exists before
# completing its build.

find_library( # Sets the name of the path variable.
              log-lib

              # Specifies the name of the NDK library that
              # you want CMake to locate.
              log )

# Specifies libraries CMake should link to your target library. You
# can link multiple libraries, such as libraries you define in this
# build script, prebuilt third-party libraries, or system libraries.

target_link_libraries( # Specifies the target library.
                       RenderingPlugin

                       lib_opencv

                       # Links the target library to the log library
                       # included in the NDK.
                       ${log-lib} )

