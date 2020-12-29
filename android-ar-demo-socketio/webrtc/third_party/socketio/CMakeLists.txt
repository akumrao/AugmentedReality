# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.7)

project(socketio)

#set(CMAKE_C_COMPILER "clang")
#set(CMAKE_CXX_COMPILER clang++)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED True)



message( ${CMAKE_CURRENT_SOURCE_DIR} )
message( ${CMAKE_CURRENT_BINARY_DIR} )
message( ${CMAKE_SYSTEM_NAME} )


set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include_directories(${WEBRTC_REPO}/socketio/client/src/base/include ${WEBRTC_REPO}/socketio/client/src/http_parser
        ${WEBRTC_REPO}/socketio/client/src/crypto/include ${WEBRTC_REPO}/socketio/client/src/json/include
        ${WEBRTC_REPO}/socketio/client/src/net/include ${WEBRTC_REPO}/socketio/client/src/http/include
        ${WEBRTC_REPO}/socketio/client/src/libuv/include ${WEBRTC_REPO}/socketio/client/src/libuv/src
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix ${WEBRTC_REPO}/socketio/client/src/signal/include
        ${WEBRTC_REPO}/third_party/boringssl/src/include)

file(GLOB SOURCES "${WEBRTC_REPO}/socketio/S*.cpp"  "${WEBRTC_REPO}/socketio/client/src/libuv/src/*.cpp" "${WEBRTC_REPO}/socketio/client/src/base/src/*.cpp" "${WEBRTC_REPO}/socketio/client/src/net/src/*.cpp"  "${WEBRTC_REPO}/socketio/client/src/http/src/*.cpp" "${WEBRTC_REPO}/socketio/client/src/http_parser/*.cpp" "${WEBRTC_REPO}/socketio/client/src/json/src/*.cpp" "${WEBRTC_REPO}/socketio/client/src/signal/src/*.cpp" )

list(APPEND SOURCES
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/async.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/core.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/dl.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/fs.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/getaddrinfo.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/getnameinfo.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/loop-watcher.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/loop.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/pipe.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/poll.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/process.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/random-devurandom.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/signal.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/stream.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/tcp.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/thread.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/tty.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/udp.cpp)



 if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
  list(APPEND uv_defines _GNU_SOURCE _POSIX_C_SOURCE=200112)
  list(APPEND SOURCES
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-core.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-inotify.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-syscalls.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/proctitle.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/random-getrandom.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/random-sysctl.cpp)
  endif()

if(CMAKE_SYSTEM_NAME STREQUAL "Android")
    list(APPEND uv_libs dl)
    list(APPEND SOURCES
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/android-ifaddrs.cpp
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-core.cpp
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-inotify.cpp
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/linux-syscalls.cpp
        #${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/procfs-exepath.cpp
        ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/pthread-fixes.cpp
        #${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/sysinfo-loadavg.cpp
            )
endif()

if(APPLE OR CMAKE_SYSTEM_NAME MATCHES "DragonFly|FreeBSD|NetBSD|OpenBSD")
  list(APPEND SOURCES ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/bsd-ifaddrs.cpp ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/kqueue.cpp)
endif()

if(APPLE OR CMAKE_SYSTEM_NAME MATCHES "Android|Linux|OS/390")
  list(APPEND SOURCES ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/proctitle.cpp)
endif()


if(APPLE OR CMAKE_SYSTEM_NAME STREQUAL "OpenBSD")
  list(APPEND SOURCES ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/random-getentropy.cpp)
endif()

if(APPLE)
  list(APPEND uv_defines _DARWIN_UNLIMITED_SELECT=1 _DARWIN_USE_64_BIT_INODE=1)
  list(APPEND SOURCES
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/darwin-proctitle.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/darwin.cpp
       ${WEBRTC_REPO}/socketio/client/src/libuv/src/unix/fsevents.cpp)
endif()


#
#file(GLOB SOURCES "*.cpp")

#set (CMAKE_CXX_STANDARD 14)
#
#set(CMAKE_BUILD_TYPE Debug)
#
#set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -g")

# Creates and names a library, sets it as either STATIC
# or SHARED, and provides the relative paths to its source code.
# You can define multiple libraries, and CMake builds them for you.
# Gradle automatically packages shared libraries with your APK.

add_library( # Sets the name of the library.
        socketio

        # Sets the library as a shared library.
        STATIC

        # Provides a relative path to your source file(s).
        ${SOURCES})

set_target_properties(socketio PROPERTIES LINKER_LANGUAGE CXX)

target_compile_definitions(socketio
                           PUBLIC ${uv_defines} )

# Searches for a specified prebuilt library and stores the path as a
# variable. Because CMake includes system libraries in the search path by
# default, you only need to specify the name of the public NDK library
# you want to add. CMake verifies that the library exists before
# completing its build.

#add_executable(main  main.cpp)

#target_link_libraries(main  ${CMAKE_CURRENT_BINARY_DIR}/libsocketio.a)
#target_link_libraries(main dl pthread ssl crypto )

#target_link_libraries (testme LINK_PUBLIC ${SOURCES})
