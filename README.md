# b-sabr
B-SABR: Blocking SAM &amp; AVR Build Runner.

B-SABR is a utility for building Atmel Studio/Microchip Studio solutions at the command line. This utility exists because Atmel Studio/Microchip Studio:

* does not support blocking builds (i.e. the invoking executable staying alive until the build is finished)
* does not support directing build output to the console

Both of these are desirable traits in a build executable for various reasons and use cases.

## How to use B-SABR
B-SABR is a minimalist wrapper for ```AtmelStudio.exe```. Therefore, it runs without interaction, and supports a very simple command line argument format.

```
Usage: bsabr.exe studio_path studio_arguments
    studio_path: the path to AtmelStudio.exe
    studio_arguments: the build arguments to pass to AtmelStudio.exe
```

If you are unsure of which arguments you should use for AtmelStudio.exe, launch it from the command line with the "/?" argument to see its usage.

### An example usage of B-SABR
```bsabr.exe "C:\Program Files (x86)\Atmel\Studio\7.0\AtmelStudio.exe" "C:\code\asf_test\asf_test.atsln" /build DEBUG```

## Help, I'm not seeing any build output!
Based on the information found in [this AvrFreaks thread](https://www.avrfreaks.net/forum/see-complete-log-command-line-build), make sure your build verbosity is appropriately set. If you haven't run the AtmelStudio.exe GUI yet, do so. If you cannot run the AtmelStudio.exe GUI, then set the appropriate registry values as specified in [this post](https://www.avrfreaks.net/comment/2874201#comment-2874201).

## Intellectual Property Notice
AVR, SAM, Atmel Studio, and Microchip Studio are all trademarks of Microchip Technology, Inc.
