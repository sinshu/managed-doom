# Managed Doom

![Managed Doom](screenshots/window.png)

The goal of this project is to make a fully functional Doom source port written in C#.

I'm currently working on the software renderer. The screenshots below show the progress so far.



## Screenshots

![E1M1 start](screenshots/doom-e1m1-start.png)

![E1M1 outside](screenshots/doom-e1m1-outside.png)

![A fake 3D bridge in Requiem MAP13](screenshots/requiem-map13-bridge1.png)

![Another 3D bridge](screenshots/requiem-map13-bridge2.png)

![Slime trail](screenshots/doom-e1m1-slime.png)



# Todo

- Software renderer  
    * [x] Front-to-back rendering using BSP
    * [x] Wall texture mapping
    * [x] Ceiling / floor texture mapping
    * [x] Transparent textures
    * [x] Diminishing lighting
    * [x] Sky rendering
    * [ ] Sprite rendering
    * [ ] High resolution rendering
    * [ ] Optimization and benchmark

- Gaming code
    * [ ] Lots of things

- Audio
    * [ ] Lots of things



## References

- __The original source code by id Software__  
https://github.com/id-Software/DOOM

- __The Game Engine Black Book: DOOM by Fabien Sanglard__  
If you want to understand the big picture of the rendering process in Doom, buy this one.  
http://fabiensanglard.net/gebbdoom/

- __The Unofficial Doom Specs__  
http://www.gamers.org/dhs/helpdocs/dmsp1666.html

- __Doom Wiki__  
https://doomwiki.org/wiki/Entryway

- __SFML - Simple and Fast Multimedia Library__  
https://www.sfml-dev.org
