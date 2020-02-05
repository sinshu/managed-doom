# Managed Doom

![Managed Doom](screenshots/window.png)

The goal of this project is to make a fully functional Doom source port written in C# with no unsafe code.

The rendering engine is almost complete and capable of showing 3D view like the screenshots below at pratical speed.

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
    * [x] Front-to-back rendering using BSP
    * [x] Wall texture mapping
    * [x] Ceiling / floor texture mapping
    * [x] Transparent textures
    * [x] Diminishing lighting
    * [x] Sky rendering
    * [x] Sprite rendering
    * [x] High resolution rendering
    * [x] Optimization

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
