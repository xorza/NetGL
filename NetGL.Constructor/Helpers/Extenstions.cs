using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using NetGL.Constructor.Infrastructure;
using NetGL.Core.Mathematics;
using Control = System.Windows.Controls.Control;

internal static class Extenstions {
    public static Vector3 AsVector3(this Color clr) {
        return new Vector3() {
            X = clr.R / 255f,
            Y = clr.G / 255f,
            Z = clr.B / 255f
        };
    }
    public static Vector4 AsVector4(this Color clr) {
        return new Vector4() {
            X = clr.R / 255f,
            Y = clr.G / 255f,
            Z = clr.B / 255f,
            W = clr.A / 255f
        };
    }
    public static Vector3 AsVector3(this System.Drawing.Color clr) {
        return new Vector3() {
            X = clr.R / 255f,
            Y = clr.G / 255f,
            Z = clr.B / 255f
        };
    }
    public static Vector4 AsVector4(this System.Drawing.Color clr) {
        return new Vector4() {
            X = clr.R / 255f,
            Y = clr.G / 255f,
            Z = clr.B / 255f,
            W = clr.A / 255f
        };
    }

    public static Vector2 AsVector2(this Point p) {
        return new Vector2(p.X, p.Y);
    }

    public static Color AsColor1(this Vector3 v) {
        return new Color() {
            R = (byte)(v.X * 255),
            G = (byte)(v.Y * 255),
            B = (byte)(v.Z * 255),
            A = 255
        };
    }
    public static System.Drawing.Color AsColor2(this Vector3 v) {
        return System.Drawing.Color.FromArgb(255, (int)(v.X * 255), (int)(v.Y * 255), (int)(v.Z * 255));
    }
    public static Color AsColor1(this Vector4 v) {
        return new Color() {
            R = (byte)(v.X * 255),
            G = (byte)(v.Y * 255),
            B = (byte)(v.Z * 255),
            A = (byte)(v.W * 255)
        };
    }
    public static System.Drawing.Color AsColor2(this Vector4 v) {
        return System.Drawing.Color.FromArgb((int)(v.W * 255), (int)(v.X * 255), (int)(v.Y * 255), (int)(v.Z * 255));
    }

    public static Brush AsBrush(this Vector4 v) {
        return new SolidColorBrush(v.AsColor1());
    }
    public static Brush AsBrush(this Vector3 v) {
        return new SolidColorBrush(v.AsColor1());
    }

    public static IWin32Window ToWinFormsWindow(this Window wnd) {
        return new Win32Window(wnd);
    }
    public static Window GetWindow(this Control control) {
        return Window.GetWindow(control);
    }

    public static bool IsNumeric(this Type type) {
        if (type == null)
            return false;

        switch (Type.GetTypeCode(type)) {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            case TypeCode.Object:
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return IsNumeric(Nullable.GetUnderlyingType(type));
                return false;
        }
        return false;
    }

    public static Vector3 Rgb2Hsb(this Vector3 rgb) {
        Vector3 hsb;
        float min, max, delta;
        min = Math.Min(Math.Min(rgb.X, rgb.Y), rgb.Z);
        max = Math.Max(Math.Max(rgb.X, rgb.Y), rgb.Z);
        hsb.Z = max;				// v
        delta = max - min;
        if (max != 0)
            hsb.Y = delta / max;		// s
        else {
            // r = g = b = 0		// s = 0, v is undefined
            hsb.Y = 0;
            hsb.X = -1;
            return hsb;
        }
        if (rgb.X == max)
            hsb.X = (rgb.Y - rgb.Z) / delta;		// between yellow & magenta
        else if (rgb.Y == max)
            hsb.X = 2 + (rgb.Z - rgb.X) / delta;	// between cyan & yellow
        else
            hsb.X = 4 + (rgb.X - rgb.Y) / delta;	// between magenta & cyan
        hsb.X /= 6;				// degrees
        if (hsb.X < 0)
            hsb.X += 1;

        return hsb;
    }
    public static Vector4 Rgba2Hsba(this Vector4 rgba) {
        return new Vector4(Rgb2Hsb((Vector3)rgba), rgba.W);
    }
    public static Vector4 Hsba2Rgba(this Vector4 hsba) {
        return new Vector4(Hsb2Rgb((Vector3)hsba), hsba.W);
    }
    public static Vector3 Hsb2Rgb(this Vector3 hsb) {
        int i;
        float f, p, q, t;
        if (hsb.Y == 0)
            // achromatic (grey)
            return new Vector3(hsb.Z);
        hsb.X *= 6;			// sector 0 to 5
        i = (int)MathF.Floor(hsb.X);
        f = hsb.X - i;			// factorial part of h
        p = hsb.Z * (1 - hsb.Y);
        q = hsb.Z * (1 - hsb.Y * f);
        t = hsb.Z * (1 - hsb.Y * (1 - f));
        switch (i) {
            case 0:
                return new Vector3(hsb.Z, t, p);
            case 1:
                return new Vector3(q, hsb.Z, p);
            case 2:
                return new Vector3(p, hsb.Z, t);
            case 3:
                return new Vector3(p, q, hsb.Z);
            case 4:
                return new Vector3(t, p, hsb.Z);
            default:		// case 5:
                return new Vector3(hsb.Z, p, q);
        }
    }

    public static string UseFormat(this string format, object arg0) {
        return string.Format(format, arg0);
    }
    public static string UseFormat(this string format, object arg0, object arg1) {
        return string.Format(format, arg0, arg1);
    }
    public static string UseFormat(this string format, object arg0, object arg1, object arg2) {
        return string.Format(format, arg0, arg1, arg2);
    }
    public static string UseFormat(this string format, params object[] args) {
        return string.Format(format, args);
    }
}