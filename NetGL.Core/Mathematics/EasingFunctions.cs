namespace NetGL.Core.Mathematics {
    public delegate float EasingFunctionDelegate(float time, float start, float delta, float duration);

    public enum EasingType {
        Linear = 0,

        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,

        CubicIn,
        CubicOut,
        CubicInOut,

        QuarticIn,
        QuarticOut,
        QuarticInOut,

        QuinticIn,
        QuinticOut,
        QuinticInOut,

        SinusoidalIn,
        SinusoidalOut,
        SinusoidalInOut,

        ExponentialIn,
        ExponentialOut,
        ExponentialInOut,

        CircularIn,
        CircularOut,
        CircularInOut
    }

    public static class EasingFunctions {
        private static readonly EasingFunctionDelegate[] _easingFunctions = new EasingFunctionDelegate[22];

        static EasingFunctions() {
            _easingFunctions[(int)EasingType.Linear] = Linear;

            _easingFunctions[(int)EasingType.QuadraticIn] = QuadraticIn;
            _easingFunctions[(int)EasingType.QuadraticOut] = QuadraticOut;
            _easingFunctions[(int)EasingType.QuadraticInOut] = QuadraticInOut;

            _easingFunctions[(int)EasingType.CubicIn] = CubicIn;
            _easingFunctions[(int)EasingType.CubicOut] = CubicOut;
            _easingFunctions[(int)EasingType.CubicInOut] = CubicInOut;

            _easingFunctions[(int)EasingType.QuarticIn] = QuarticIn;
            _easingFunctions[(int)EasingType.QuarticOut] = QuarticOut;
            _easingFunctions[(int)EasingType.QuarticInOut] = QuarticInOut;

            _easingFunctions[(int)EasingType.QuinticIn] = QuinticIn;
            _easingFunctions[(int)EasingType.QuinticOut] = QuinticOut;
            _easingFunctions[(int)EasingType.QuinticInOut] = QuinticInOut;

            _easingFunctions[(int)EasingType.SinusoidalIn] = SinusoidalIn;
            _easingFunctions[(int)EasingType.SinusoidalOut] = SinusoidalOut;
            _easingFunctions[(int)EasingType.SinusoidalInOut] = SinusoidalInOut;

            _easingFunctions[(int)EasingType.ExponentialIn] = ExponentialIn;
            _easingFunctions[(int)EasingType.ExponentialOut] = ExponentialOut;
            _easingFunctions[(int)EasingType.ExponentialInOut] = ExponentialInOut;

            _easingFunctions[(int)EasingType.CircularIn] = CircularIn;
            _easingFunctions[(int)EasingType.CircularOut] = CircularOut;
            _easingFunctions[(int)EasingType.CircularInOut] = CircularInOut;
        }

        public static EasingFunctionDelegate Get(EasingType easingType) {
            return _easingFunctions[(int)easingType];
        }

        public static float Linear(float time, float start, float delta, float duration) {
            return delta * time / duration + start;
        }

        public static float QuadraticIn(float time, float b, float c, float d) {
            time /= d;
            return c * time * time + b;
        }
        public static float QuadraticOut(float time, float b, float c, float d) {
            time /= d;
            return -c * time * (time - 2) + b;
        }
        public static float QuadraticInOut(float time, float b, float c, float d) {
            time /= d / 2;
            if (time < 1) return c / 2 * time * time + b;
            time--;
            return -c / 2 * (time * (time - 2) - 1) + b;
        }

        public static float CubicIn(float t, float b, float c, float d) {
            t /= d;
            return c * t * t * t + b;
        }
        public static float CubicOut(float time, float b, float c, float d) {
            time /= d;
            time--;
            return c * (time * time * time + 1) + b;
        }
        public static float CubicInOut(float time, float b, float c, float d) {
            time /= d / 2;
            if (time < 1) return c / 2 * time * time * time + b;
            time -= 2;
            return c / 2 * (time * time * time + 2) + b;
        }

        public static float QuarticIn(float time, float b, float c, float d) {
            time /= d;
            return c * time * time * time * time + b;
        }
        public static float QuarticOut(float time, float b, float c, float d) {
            time /= d;
            time--;
            return -c * (time * time * time * time - 1) + b;
        }
        public static float QuarticInOut(float t, float b, float c, float d) {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t + b;
            t -= 2;
            return -c / 2 * (t * t * t * t - 2) + b;
        }

        public static float QuinticIn(float time, float b, float c, float d) {
            time /= d;
            return c * time * time * time * time * time + b;
        }
        public static float QuinticOut(float time, float b, float c, float d) {
            time /= d;
            time--;
            return c * (time * time * time * time * time + 1) + b;
        }
        public static float QuinticInOut(float t, float b, float c, float d) {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t * t * t + 2) + b;
        }

        public static float SinusoidalIn(float time, float b, float c, float d) {
            return -c * MathF.Cos(time / d * (MathF.PI / 2)) + c + b;
        }
        public static float SinusoidalOut(float time, float b, float c, float d) {
            return c * MathF.Sin(time / d * (MathF.PI / 2)) + b;
        }
        public static float SinusoidalInOut(float time, float b, float c, float d) {
            return -c / 2 * (MathF.Cos(MathF.PI * time / d) - 1) + b;
        }

        public static float ExponentialIn(float time, float b, float c, float d) {
            return c * MathF.Pow(2, 10 * (time / d - 1)) + b;
        }
        public static float ExponentialOut(float time, float b, float c, float d) {
            return c * (-MathF.Pow(2, -10 * time / d) + 1) + b;
        }
        public static float ExponentialInOut(float time, float b, float c, float duration) {
            time /= duration / 2;
            if (time < 1) return c / 2 * MathF.Pow(2, 10 * (time - 1)) + b;
            time--;
            return c / 2 * (-MathF.Pow(2, -10 * time) + 2) + b;
        }

        public static float CircularIn(float time, float b, float c, float duration) {
            time /= duration;
            return -c * (MathF.Sqrt(1 - time * time) - 1) + b;
        }
        public static float CircularOut(float time, float b, float c, float duration) {
            time /= duration;
            time--;
            return c * MathF.Sqrt(1 - time * time) + b;
        }
        public static float CircularInOut(float time, float b, float c, float duration) {
            time /= duration / 2;
            if (time < 1) return -c / 2 * (MathF.Sqrt(1 - time * time) - 1) + b;
            time -= 2;
            return c / 2 * (MathF.Sqrt(1 - time * time) + 1) + b;
        }
    }
}
