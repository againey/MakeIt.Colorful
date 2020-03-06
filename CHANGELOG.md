# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and starting from v2.0.0, this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2017-01-14

Coincided with release of Make It Random v2.0.

### Added

* Common color constants for each color space.
* Opacity functions to easily get opacity-altered variants of a color.
* Implemented `IEquatable<T>`.
* Implemented `IComparable<T>`, using a lexicographic ordering.
* Comparison operators `<`, `<=`, `>`, and `=>` using a lexicographic ordering.
* Functions to compute common color attributes for all color types:
  * hue
  * intensity
  * value
  * lightness
  * luma (apparent brightness)
* `GetValid(chromaBias)` for chroma-based color types, to choose the degree to which chroma or luminance is preserved when correcting invalid colors.

## [1.0.0] - 2016-11-18

Coincided with release of Make It Tiled v2.0 and Make It Random v1.0.

### Added

#### Color Spaces

* HSV (hue, saturation, value)
* HCV (hue, chroma, value)
* HSL (hue, saturation, lightness)
* HCL (hue, chroma, lightness)
* HSY (hue, saturation, luma)
* HCY (hue, chroma, luma)
* CMY (cyan, magenta, yellow)
* CMYK (cyan, magenta, yellow, key)

#### Features

* implicit conversion to/from RGB (`UnityEngine.Color`)
* explicit conversion to/from any other color type
* explicit conversion to/from `Vector3`/`Vector4`
* color channel indexing
* linear interpolation (cyclically aware for hue)
* arithmetic operators
* equality operators
* string formattable
* color space boundary validation and correction
* awareness of canonical and non-canonical color representations

#### Demos

* color space volumes and linear gradients
