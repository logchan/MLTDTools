﻿namespace MillionDance.Core {
    internal sealed class ConversionConfig {

        private ConversionConfig() {
        }

        public MotionFormat MotionFormat { get; private set; }

        // PMX

        public bool ScaleToPmxSize { get; private set; }

        public bool TranslateBoneNamesToMmd { get; private set; }

        public bool AppendIKBones { get; private set; }

        public bool FixMmdCenterBones { get; private set; }

        public bool AppendEyeBones { get; private set; }

        public bool HideUnityGeneratedBones { get; private set; }

        public SkeletonFormat SkeletonFormat { get; private set; }

        public bool TranslateFacialExpressionNamesToMmd { get; private set; }

        // VMD

        public bool Transform60FpsTo30Fps { get; private set; }

        public bool ScaleToVmdSize { get; private set; }

        private static readonly ConversionConfig UseMltdMotion = new ConversionConfig {
            MotionFormat = MotionFormat.Mltd,
            ScaleToPmxSize = true,
            TranslateBoneNamesToMmd = true,
            AppendIKBones = false,
            FixMmdCenterBones = false,
            AppendEyeBones = false,
            HideUnityGeneratedBones = true,
            SkeletonFormat = SkeletonFormat.Mltd,
            TranslateFacialExpressionNamesToMmd = true,
            Transform60FpsTo30Fps = true,
            ScaleToVmdSize = true
        };

        private static readonly ConversionConfig UseMmdMotion = new ConversionConfig {
            MotionFormat = MotionFormat.Mmd,
            ScaleToPmxSize = true,
            TranslateBoneNamesToMmd = true,
            AppendIKBones = true,
            FixMmdCenterBones = true,
            AppendEyeBones = true,
            HideUnityGeneratedBones = true,
            SkeletonFormat = SkeletonFormat.Mmd,
            TranslateFacialExpressionNamesToMmd = true,
            Transform60FpsTo30Fps = true,
            ScaleToVmdSize = true
        };

        public static ConversionConfig Current { get; } = UseMltdMotion;

    }
}
