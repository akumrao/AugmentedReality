cmake_minimum_required(VERSION 3.7)

add_definitions(-D_GNU_SOURCE -DHAVE_SYS_UIO_H -DANDROID_NDK )

remove_definitions(-D__GNUC__ )

# YASM assembly is only checked to be working on Windows and Linux.
# Mac is known to fail certain tests when building, but actual assembly
# is believed to work.
# MSAN builds are flaky with assembler. crbug.com/685168
#use_assembler = (is_win || is_linux) &&
#                (target_cpu == "x86" || target_cpu == "x64") && !is_msan

# This IF statement will make the targets visible only on specific builds,
# which will lead to failures on other platforms if accidentally invoked.
#if (use_assembler) {
#  yasm_defines = []
#  if (!is_component_build) {
#    if (is_mac || is_ios) {
#      yasm_defines += [ "WELS_PRIVATE_EXTERN=:private_extern" ]
#    } else if (is_linux || is_android || is_fuchsia) {
#      yasm_defines += [ "WELS_PRIVATE_EXTERN=:hidden" ]
#    }
#  }


project( openh264 )

message("CMAKE_SOURCE_DIR=${CMAKE_SOURCE_DIR}")

include_directories(encoder PUBLIC 

  ${WEBRTC_REPO}/third_party/openh264/src/codec/api/svc
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/arm/
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/x86
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/plus/inc
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/plus/src
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/interface
  ${WEBRTC_REPO}/third_party/android_ndk/sources/android/cpufeatures


  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/interface
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/adaptivequantization
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/backgrounddetection
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/complexityanalysis
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/denoise
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/downsample
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/imagerotate
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scenechangedetection
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scrolldetection
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/vaacalc
  ${WEBRTC_REPO}/third_party/android_ndk/sources/android/cpufeatures


)

set(openh264_SRCS

 ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsList.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsLock.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsTask.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsTaskThread.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsThread.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsThreadLib.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/WelsThreadPool.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/copy_mb.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/cpu.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/cpu_core.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/crt_util_safe_x.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/deblocking_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/expand_pic.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/golomb_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/intra_pred_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/ls_defines.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/macros.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/mc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/measure_time.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/memory_align.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/sad_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/typedefs.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/utils.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/version.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/welsCodecTrace.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/wels_common_defs.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/inc/wels_const_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/WelsTaskThread.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/WelsThread.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/WelsThreadLib.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/WelsThreadPool.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/common_tables.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/copy_mb.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/cpu.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/crt_util_safe_x.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/deblocking_common.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/expand_pic.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/intra_pred_common.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/mc.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/memory_align.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/sad_common.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/utils.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/common/src/welsCodecTrace.cpp

  ${WEBRTC_REPO}/third_party/android_ndk/sources/android/cpufeatures/cpu-features.c





  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/interface/IWelsVP.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/adaptivequantization/AdaptiveQuantization.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/adaptivequantization/AdaptiveQuantization.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/backgrounddetection/BackgroundDetection.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/backgrounddetection/BackgroundDetection.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/WelsFrameWork.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/WelsFrameWork.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/WelsFrameWorkEx.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/memory.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/memory.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/resource.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/typedef.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/common/util.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/complexityanalysis/ComplexityAnalysis.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/complexityanalysis/ComplexityAnalysis.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/denoise/denoise.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/denoise/denoise.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/denoise/denoise_filter.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/downsample/downsample.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/downsample/downsample.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/downsample/downsamplefuncs.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/imagerotate/imagerotate.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/imagerotate/imagerotate.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/imagerotate/imagerotatefuncs.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scenechangedetection/SceneChangeDetection.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scenechangedetection/SceneChangeDetection.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scrolldetection/ScrollDetection.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scrolldetection/ScrollDetection.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scrolldetection/ScrollDetectionFuncs.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/scrolldetection/ScrollDetectionFuncs.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/vaacalc/vaacalcfuncs.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/vaacalc/vaacalculation.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/processing/src/vaacalc/vaacalculation.h









  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/as264_common.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/au_set.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/deblocking.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/decode_mb_aux.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/dq_map.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/encode_mb_aux.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/encoder.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/encoder_context.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/extern.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/get_intra_predictor.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/mb_cache.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/md.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/mt_defs.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/mv_pred.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/nal_encap.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/param_svc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/parameter_sets.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/paraset_strategy.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/picture.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/picture_handle.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/rc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/ref_list_mgr_svc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/sample.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/set_mb_syn_cabac.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/set_mb_syn_cavlc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/slice.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/slice_multi_threading.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/stat.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_base_layer_md.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_enc_frame.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_enc_golomb.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_enc_macroblock.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_enc_slice_segment.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_encode_mb.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_encode_slice.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_mode_decision.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_motion_estimate.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_set_mb_syn.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/svc_set_mb_syn_cavlc.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/vlc_encoder.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_common_basis.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_const.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_func_ptr_def.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_preprocess.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_task_base.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_task_encoder.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_task_management.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/inc/wels_transpose_matrix.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/au_set.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/deblocking.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/decode_mb_aux.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/encode_mb_aux.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/encoder.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/encoder_data_tables.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/encoder_ext.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/get_intra_predictor.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/md.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/mv_pred.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/nal_encap.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/paraset_strategy.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/picture_handle.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/ratectl.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/ref_list_mgr_svc.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/sample.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/set_mb_syn_cabac.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/set_mb_syn_cavlc.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/slice_multi_threading.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_base_layer_md.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_enc_slice_segment.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_encode_mb.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_encode_slice.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_mode_decision.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_motion_estimate.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_set_mb_syn_cabac.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/svc_set_mb_syn_cavlc.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/wels_preprocess.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/wels_task_base.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/wels_task_encoder.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/core/src/wels_task_management.cpp
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/plus/inc/welsEncoderExt.h
  ${WEBRTC_REPO}/third_party/openh264/src/codec/encoder/plus/src/welsEncoderExt.cpp

  # Note: Purposefully excluded: 'src/codec/encoder/plus/src/DllEntry.cpp',
  # This file is not built by the OpenH264 original build files.

)

add_library(openh264 STATIC ${openh264_SRCS})


#add_library(myopenh264 INTERFACE)
#target_link_libraries(myopenh264 INTERFACE common processing encoder)