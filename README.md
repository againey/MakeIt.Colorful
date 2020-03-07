# MakeIt.Colorful

MakeIt.Colorful is a collection of color types to enable working with a variety
of color spaces beyond just RGB. Color components include;
* hue and CMY (subtractive RGB)
* saturation and chroma
* value, lightness, luma, and key (ink-based blackness)

## Getting Started

### Prerequisites

MakeIt.Colorful depends on the following Unity packags:

* [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest/)

### Installing

This repository is arranged as a valid Unity package that can be imported into a
Unity project using the instructions in [the Unity Package Manager Manual](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

### How MakeIt.Colorful Is Organized

MakeIt.Colorful is a collection of color types for use in your Unity scripts.
Following the pattern provided by `UnityEngine.Color`, these types are structs
exposing their various color component fields, while also providing some useful
methods and operators.

Each type represents colors within a particular color space, making it simple to
work with colors within that color space. In addition, conversion functions
exist to convert from any of the available color spaces to any other, including
the RGB color space as represented by `UnityEngine.Color`.

### Using MakeIt.Colorful

Because Unity itself and so many other scripts and assets only recognize
`UnityEngine.Color`, using the color spaces in MakeIt.Colorful will often
involve either convert to or from `UnityEngine.Color`, or both.

For example, here is an easy way to get the RGB representation of a particular
hue:
```
Color rgb = (Color)new ColorHSV(hue, 1f, 1f);
```

Linear interpolation between colors is included, which will often produce
different and more desirable results in other color spaces than when
interpolating within the RGB color space. The hue-based color spaces will avoid
muddy gray middle sections, for example, and the HCY color space in particular
provides good interpolation of apparent vibrance and brightness, whereas other
color spaces will often produce apparent banding.
```
ColorHCY hcy = ColorHCY.Lerp(ColorHCY.FromRGB(1f, 0f, 0f), ColorHCY.FromRGB(0f, 0.2f, 0.3f), 0.354f);
```

Lastly, there are methods to manage the bounds of a color space and the
uniqueness of individual colors. The non-HDR RGB color space has a range of
\[0, 1\] for each color channel, and the other color spaces have corresponding
ranges such that every color within these ranges will convert to an RGB color
within these standard bounds. Methods exist to check if a color is within these
bounds, and further to clamp to the nearest color within the bounds if not
already in bounds. And in some color spaces, certain colors can be expressed in
more than one way, so each color is defined to have a single canonical
representation. Thus, methods exist to check if a color is canonical, and to
convert to the canonical form if not.
```
ColorHCV hcv = new ColorHCV(0.5f, 0.6f, 0.2f, 1.1f);
Debug.Log(hcv.IsValid());         // false
Debug.Log(hcv.GetNearestValid()); // HCVA(0.500, 0.400, 0.400, 1.000)

ColorHSL hsl = new ColorHSL(0.2f, 0f, 0.3f);
Debug.Log(hsl.IsCanonical());     // false
Debug.Log(hsl.GetCanonical());    // HSLA(0.000, 0.000, 0.300, 1.000)
```

Every color space type includes an alpha channel to indicate opacity, serving
the same purpose as in `UnityEngine.Color`. When converting between color
spaces, opacity plays no role, and is simply copied directly as is. For any
constructors or similar functions that leave out opacity, it defaults to 1.

### Available Color Spaces

* ColorCMY: A subtractive color space mimicking cyan, magenta, and yellow inks.
* ColorCMYK: A subtractive color space mimicking cyan, magenta, yellow, and black (key) inks.
* ColorHSV: A hue-based color space using saturation to measure vividness and value to measure luminance.
* ColorHCV: A hue-based color space using chroma to measure vividness and value to measure luminance.
* ColorHSL: A hue-based color space using saturation to measure vividness and lightness to measure luminance.
* ColorHCL: A hue-based color space using chroma to measure vividness and lightness to measure luminance.
* ColorHSY: A hue-based color space using saturation to measure vividness and luma to measure luminance.
* ColorHCY: A hue-based color space using chroma to measure vividness and luma to measure luminance.

## Running the Tests

MakeIt.Colorful includes tests that can be executed by the [Unity Test Runner](https://docs.unity3d.com/2019.1/Documentation/Manual/testing-editortestsrunner.html).

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

MakeIt.Colorful was originally part of a collection of private projects in a
single repository, so some of that shared history still exists within this
repository. Releases for other projects before the split are also tagged, such
as `makeit-random-v1.0`.

## Authors

* **Andy Gainey**

## License

This project is licensed under the Apache License, Version 2.0 - see the [LICENSE.md](LICENSE.md) file for details
