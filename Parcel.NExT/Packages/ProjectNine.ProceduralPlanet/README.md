# Parcel.ProceduralGeneration.Planet

Useful for practical purpose. Part of Raytrix initiative.

Based on https://github.com/Charles-Zhang-Project-Nine-Experiments/ProceduralPlanet?tab=readme-ov-file.

## TODO

- [ ] Merge with Raytrix.

# Procedural Planet (C# .Net 7)

This code was derived (or a fork) from the original [planet.exe](https://github.com/Project-Nine-Tooling/planet) by [Torben Mogensen](http://hjemmesider.diku.dk/~torbenm/Planet/).

The experiment (this solution) contains three parts:

1. Part 1 - PlanetCsharp: A no-brainer migration of original C code to C#, without making any changes; Removed the ability to write to stdout as output. (The original code was writting binary to stdout?)
2. Part 2 - HDPlanet: A modification of the part 1 to suit custom needs and with custom improvements and clean up. Compared to the original, this one had the below improvements. Like the original, this is published as a single executable.
    * All variables renamed and properly scoped;
    * Removed useless std output and debug output; Remove unused variables;
    * Update naming of functions and update proper case to PascalCase; Further contain and modularize function calls; Replace some custom math functions with existing Math library functions.
    * Increased match map size to double the original resolution.
3. Part 3 - ProceduralPlanet for Project Nine: A complete rewrite of the same underlying algorithm using more native logic, [Pure](https://github.com/pure-the-Language/Pure/)-scriptable, and provides additional features. See P9 dated note on the vision on HD/Procedural Planet and the README file for the solution project. Below are the highlights:
    * (TODO) Provide `--<parameterName>` CLI argument interface with new default values.
    * (todo) Increase match map size to infinite detail
    * (todo) Refine procedural climate and ecosystem rules
    * (todo) Allow defining arbitrary custom terrain heights from external files

Notice it takes a lot of time to reconcile between PlanetCSharp and HDPlanet and we are keeping lots of old-styled code in HDPlanet despite this is a first-attempt refactor because we didn't fully understand the algorithm and wants to keep original coding style for easier code comparison for study and debugging purpose.

## TODO

We have migrated original C# re-write, and original HDPlanet improvement, but haven't got to implement a fully modular implementation for CLI/Pure/sciprting use yet (it's not done in the original re-write either) (Originally planned [ProceduralPlanet.Core](https://github.com/Charles-Zhang-Project-Nine-Experiments/ProceduralPlanet/tree/master/Generators/ProceduralPlanet.Core) and [ProceduralPlanet](https://github.com/Charles-Zhang-Project-Nine-Experiments/ProceduralPlanet/tree/master/Generators/ProceduralPlanet)). So we will need to do it here.

- [ ] Need to figure out a way to re-implement original unit tests; Consider putting them inside a dedicated Parcel NExT UnitTests folder.
- [ ] Remove the video generation aspect into a dedicated utility module so as to remove dependencies

Basic usability:

- [ ] Provide `Image` return type entry functions
- [ ] Implement writting to PnG using Parcel.Image package

## Technical Notes

Main references:

1. Original source code: http://hjemmesider.diku.dk/~torbenm/Planet/ (I have created a fork here: https://github.com/Project-Nine-Tooling/planet)
2. Original user manual documentation available in the source code ZIP file above
3. Key methodology conference slides: http://hjemmesider.diku.dk/~torbenm/Planet/PSIslides.pdf
4. Torben Mogensen also has a conference paper available on ResearchGate named "Planet Map Generation by Tetrahedral Subdivision".

# Planet C# (Original planet.exe Code (Migrated to C#))

This is a straightforward migration into C# for original code `PlanetC` in C/C++, we refrain from making significant changes and have kept the original source code style.

Below is the main doc from original single .c file:

```c
/* The program generates planet maps based on recursive spatial subdivision */
/* of a tetrahedron containing the globe. The output is a colour BMP bitmap. */
/* with options for PPM or B/W bitmaps */

/* The colours may optionally be modified according to latitude to move the */
/* icecaps lower closer to the poles, with a corresponding change in land colours. */

/* The Mercator map at magnification 1 is scaled to fit the Width */
/* it uses the full height (it could extend infinitely) */
/* The orthographic projections are scaled so the full view would use the */
/* full Height. Areas outside the globe are coloured black. */
/* Stereographic and gnomonic projections use the same scale as orthographic */
/* in the center of the picture, but distorts scale away from the center. */

/* It is assumed that pixels are square */
/* I have included procedures to print the maps as bmp (Windows) or */
/* ppm(portable pixel map) bitmaps  on standard output or specified files. */
```

```
/* Supported output file types:
    BMP - Windows Bit MaPs
    PPM - Portable Pix Maps
    XPM - X-windows Pix Maps
 */
```

# HD Planet

This is a somewhat more native C# implementation of PlanetCSharp, but we are still keeping the original interface.

TODO: 

* Check some key (state) parameters and make sure we did not make unintended modifications to rotation and generator initialization. Testing and validation with original program on all possible input argument combinations. 
* At the moment "rotation" `-T lo la` seems to have problems. This must be fixed before we can move on to ProceduralPlanet project.
* There are a bunch of variables that's not used, we are leaving them as is to keep them close to the original code, but should double check they are actually not used and add comments for ProceduralPlanet implemnetation.

## Examples

```C#
void Earth()
{
    var width = 2048;
    var height = 2048;
    var magnification = 1;
    var longitude = 15.0;
    var latitude = 0.0;
    var hGridSize = 10;
    var vGridSize = 20;
    var projection = 'p';
    var seed = 0.005;

    var arguments = $"-ps -s {seed} -m {magnification} -L {latitude} -l {longitude} -G {hGridSize} -g {vGridSize} -w {width} -h {height} -p {projection} -M 0.1 Earth.map -o EarthHD.bmp -E -b -z";
    new CommandLineParser().ParseArguments(arguments).Run();
}
```

# MapFunctions README

(Not sure what this is does, pending deleting this project, or providing as an additional CLI format to HDPlanet, or as an additional utility function to ProceduralPlanet.Core)

## Examples

```C#
// Generate zoom-ins
new ZoomIn()
{
    Longitude = 15,
    Latitude = 0,
    OutputPrefix = "Test2"
}.Run(1, 15);
```

# ProjectNine.ProceduralPlanet

Tags: Project Nine

A migration and re-implementation of https://github.com/Charles-Zhang-Project-Nine-Experiments/ProceduralPlanet, targeting specific for Pure/Parcel use.
All rights belongs to the original authors and Project Nine.