#!/bin/bash

# ffmpeg 

# Android

WEBRTC_NDK=/workspace/webrtc_android/src/third_party/android_ndk
TOOLCHAIN=${WEBRTC_NDK}/toolchains/llvm/prebuilt/linux-x86_64

./configure \
--ar=${TOOLCHAIN}/bin/arm-linux-androideabi-ar \
--as=${TOOLCHAIN}/bin/arm-linux-androideabi-as \
--cc=${TOOLCHAIN}/bin/armv7a-linux-androideabi16-clang \
--cxx=${TOOLCHAIN}/bin/armv7a-linux-androideabi16-clang++ \
--nm=${TOOLCHAIN}/bin/arm-linux-androideabi-nm \
--ranlib=${TOOLCHAIN}/bin/arm-linux-androideabi-ranlib \
--strip=${TOOLCHAIN}/bin/arm-linux-androideabi-strip \
--enable-cross-compile \
--target-os=android \
--arch=armv7a \
--disable-shared --disable-doc --disable-programs \
--enable-debug --disable-symver --disable-asm \
--disable-encoders --disable-hwaccels --disable-bsfs --disable-devices --disable-filters \
--disable-protocols --enable-protocol=file \
--disable-parsers --enable-parser=mpegaudio --enable-parser=h264 --enable-parser=hevc \
--disable-demuxers --enable-demuxer=h264 \
--disable-decoders --enable-decoder=mp3 --enable-decoder=h264 \
--prefix=`pwd`/out_armv7a && \
make -j16 install && \
