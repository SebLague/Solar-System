KinoBloom
=========

*Bloom* is an image effect for Unity that adds bloom/veiling glare effect to
rendered frames.

![Screenshot](http://i.imgur.com/GSIKfzs.png)
![Screenshot](http://i.imgur.com/q6GZV7R.png)

System Requirements
-------------------

Unity 5.5 or later versions.

Installation
------------

Download one of the unitypackage files from the [Releases] page and import it
to a project.

[Releases]: https://github.com/keijiro/KinoBloom/releases

Effect Properties
-----------------

![Inspector](http://i.imgur.com/glShkW3.png)

- **Threshold** - Filters out pixels under this level of brightness. This value
  should be given in the gamma space (as used in the color picker).

- **Soft Knee** - Makes transition between under/over-threshold gradual (0 =
  hard threshold, 1 = soft threshold).

- **Intensity** - Total intensity of the effect.

- **Radius** - Controls the extent of veiling effects. The value is not related
  to screen size and can be controlled in a resolution-independent fashion.

- **High Quality** - Controls the filter quality and the buffer resolution. On
  mobile platforms, it might strongly affect the performance of the effect,
  therefore it’s recommended to be unchecked when running on mobile devices.

- **Anti Flicker** - Sometimes the effect introduces strong flickers (flashing
  noise). This option is used to suppress them with a noise reduction filter.

License
-------

[MIT](LICENSE.md)
