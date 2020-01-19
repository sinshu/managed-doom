# Managed Doom

![Managed Doom](screenshots/window.png)

The goal of this project is to make a fully functional Doom source port written in C#.

The software renderer is almost complete and capable of showing 3D image which is almost identical to the original DOS version.

Now I'm working on the game physics, including thing movement, interaction and so on.



## Screenshots

![E1M1 start](screenshots/doom-e1m1-start.png)

![E1M1 outside](screenshots/doom-e1m1-outside.png)

![A fake 3D bridge in Requiem MAP13](screenshots/requiem-map13-bridge1.png)

![Another 3D bridge](screenshots/requiem-map13-bridge2.png)

![Slime trail](screenshots/doom-e1m1-slime.png)



## Video sample

https://www.youtube.com/watch?v=NwTEDQsBduI  

[![YouTube video of Managed Doom](https://img.youtube.com/vi/NwTEDQsBduI/0.jpg)](https://www.youtube.com/watch?v=NwTEDQsBduI)



## Todo

- __Software renderer__  
    * [x] __Front-to-back rendering using BSP__  
    Since all the special culling methods that the original version has are implemented, many cool tricks including 3D bridge, deep water and linguortal are correctly shown.
    * [x] __Wall texture mapping__
    * [x] __Ceiling / floor texture mapping__
    * [x] __Transparent textures__
    * [x] __Diminishing lighting__
    * [x] __Sky rendering__
    * [x] __Sprite rendering__
    * [x] __High resolution rendering__
    * [x] __Optimization__  
    Now the rendeing speed is comparable with other C-based ports even without unsafe code.

- __Gaming code__
    * [ ] Player movement
    * [ ] Monster movement
    * [ ] Doors and platforms
    * [ ] Lots of things

- __Audio__
    * [ ] Lots of things



## References

- __The original source code by id Software__  
https://github.com/id-Software/DOOM

- __The Game Engine Black Book: DOOM by Fabien Sanglard__  
If you want to understand the big picture of the rendering process in Doom, buy this one!  
http://fabiensanglard.net/gebbdoom/

- __The Unofficial Doom Specs by Matthew S Fell__  
http://www.gamers.org/dhs/helpdocs/dmsp1666.html

- __Doom Wiki__  
https://doomwiki.org/wiki/Entryway

- __SFML - Simple and Fast Multimedia Library__  
https://www.sfml-dev.org
