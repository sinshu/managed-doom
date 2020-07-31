# Managed Doom

![Doom II MAP01 screenshot](screenshots/doom2-map01.png)

Managed Doom is a Doom source port entirely written in C#. Based on [Linux Doom](https://github.com/id-Software/DOOM), most functionalities necessary for single player game are implemented and now it's in the playable state.

## Screenshots

![Doom E1M1](screenshots/doom-e1m1.png)

![Doom II MAP11](screenshots/doom2-map11.png)

![Mr.DooM 30 MAP29](screenshots/mrdoom30-map29.png)

![Eternal Doom MAP29](screenshots/eternal-map29.png)

![Requiem MAP13](screenshots/requiem-map13.png)

## Demo video

https://www.youtube.com/watch?v=9_tncwL7qvM  

[![Demo video](https://img.youtube.com/vi/9_tncwL7qvM/0.jpg)](https://www.youtube.com/watch?v=9_tncwL7qvM)

## Todo

* __Software renderering__  
    - [x] Front-to-back rendering using BSP
    - [x] Wall texture mapping
    - [x] Ceiling / floor texture mapping
    - [x] Transparent textures
    - [x] Diminishing lighting
    - [x] Sky rendering
    - [x] Sprite rendering
    - [x] High resolution rendering
    - [x] Optimization
    - [x] Fuzz effect
    - [x] Palette effects

* __Game__
    - [x] Collision detection
    - [x] Player movement
    - [x] Player attack
    - [x] Monster AI
    - [x] Doors and platforms
    - [x] Animated textures
    - [x] Demo compatibility (All the v1.9 IWAD demos can be played)

* __Audio__
    - [x] Sound
    - [x] Music

* __Misc__
    - [x] Status bar
    - [x] Automap
    - [x] Title screen
    - [x] Intermission screen
    - [x] Menu screen
    - [x] Save / load
    - [x] Screen melt animation
    - [x] Config
    - [ ] DeHackEd support

## License

Managed Doom is distributed under the [GPLv2 license](licenses/LICENSE_ManagedDoom.txt).  
Managed Doom uses the following libraries.

* __[SFML](https://github.com/SFML/SFML), [CSFML](https://github.com/SFML/CSFML) and [SFML.Net](https://github.com/SFML/SFML.Net) by Laurent Gomila ([zlib license](licenses/LICENSE_SFML.txt))__
* __[C# Synth](https://archive.codeplex.com/?p=csharpsynthproject) by Alex Veltsistas ([MIT license](licenses/LICENSE_CSharpSynth.txt))__
* __[TimGM6mb](https://musescore.org/en/handbook/soundfonts-and-sfz-files) by Tim Brechbill ([GPLv2 license](licenses/LICENSE_TimGM6mb.txt))__

SFML uses the following libraries.

* __FreeType ([GPLv2 license](licenses/LICENSE_FreeType.txt))__
* __libjpeg (public domain)__
* __stb_image (public domain)__
* __OpenAL Soft ([LGPL license](licenses/LICENSE_OpenALSoft.txt))__
* __libogg ([BSD-3 license](licenses/LICENSE_libogg.txt))__
* __libvorbis ([BSD-3 license](licenses/LICENSE_libvorbis.txt))__
* __libFLAC ([BSD-3 license](licenses/LICENSE_libFRAC.txt))__

## References

* __The Game Engine Black Book: DOOM by Fabien Sanglard__  
If you want to understand the big picture of the rendering process in Doom, buy this one.  
https://fabiensanglard.net/gebbdoom/index.html

* __The Unofficial Doom Specs by Matthew S Fell__  
http://www.gamers.org/dhs/helpdocs/dmsp1666.html

* __MUS File Format Description by Vladimir Arnost__  
https://www.doomworld.com/idgames/docs/editing/mus_form

* __Chocolate Doom by Simon Howard__  
Chocolate Doom is used as the reference of  compatibility tests.  
https://github.com/chocolate-doom/chocolate-doom

* __Crispy Doom by Fabian Greffrath__  
The minimal HUD is imported from Crispy Doom.  
https://github.com/fabiangreffrath/crispy-doom

* __Doom Wiki__  
https://doomwiki.org/wiki/Entryway
