BUG: For the spout2 source to change the plugin window has to be open in the current version. 
I personally hide it in the bottom left corner!
![VNyan_HboVJBGaKT](https://github.com/user-attachments/assets/4261c852-d4a6-467f-8e9a-898fd26471e2)


------------------------------------------------

![image](https://raw.githubusercontent.com/Sjatar/StylisticScreenLight/main/examplegif.webp)

This plugin for VNyan will shade your character based on a spout texture sent from for example OBS.

I have not found this plugin to affect performance on my system but this can be affected by framerates and texture sizes! 
I try my best to make my plugins efficent and this should run on low end systems.

------------------------------------------------

To install the plugin, simply extract all files in the [release](https://github.com/Sjatar/StylisticScreenLight/releases) zip into: 
- Items/Assemblies

This is located where you have intalled VNyan.

------------------------------------------------

Make sure you have spout 2 plugin installed to OBS, I recommend and use: https://github.com/Off-World-Live/obs-spout2-plugin/

Make sure you have "Allow 3rd Party mods/Plugins" enabled in VNyan!
![VNyan_kbObNAFUiE](https://github.com/user-attachments/assets/30801d46-0624-4cbb-815e-fecac2bfdd0c)

-----------------------------------------------

Open OBS! For this to work you need spout2 installed. If you have a game/display or window capture, right click it and add a Spout filter! In the example I added a browser source.

This plugin supports any spout name which can be changed through the plugin window. Default value is "screen".

![obs64_pOcIbsJY4t](https://github.com/user-attachments/assets/fdc6d134-c37d-499b-8e43-6cd24d002421)

----------------------------------------------

This plugin comes with a custom window to set values in! It is recomended to use a node graph to set these values automatically. 
But the UI window makes it very easy to troubleshoot and test settings.

I will give a fast explanation of each setting.
- Effect on/off: turns the entire plugin on or off.
- High Light Strength: Affects the rim light effect.
- High Light X-offset: Positive values create a larger rim light on the top of the character.
- High Light Y-offset: Positive values create a larger rim light on the left side of the character.
- High Light Gaussian Quality: Affects the blur quality of the highlight.
- High Light Gaussian Iterations: Affects the number of iteration the blur is done. Higher value increases the size and quality.
- Base Light Strength: This affects the characters colour/light without the effect.
- Overlay Light Strength: Ontop of a rim light the entire character is shaded as well. This affects the strength of this light.
- Overlay Light Kawase Iterations: We use a kawase blur to blur the input texture. Higher iteration increases the blur amount.
- Brightness Threshold Filter: This prevents white lights from overpowering all other colour.
- Spout2 Source Name: The name of the spout2 texture input.
- Spout2 Source Width: The width of the input texture.
- Spout2 Source Height: The height of the input texture.
- Spout2 Source X Offset: The spout2 texture offset from the left of the OBS canvas. 
- Spout2 Source Y Offset: The spout2 texture offset from the bottom of the OBS canvas. 
- VNyan Source X Offset: OBS VNyan capture offset from the left of the OBS canvas.
- VNyan Source Y Offset: OBS VNyan capture offset from the bottom of the OBS canvas.

![image](https://github.com/user-attachments/assets/6a0a90f1-c0b7-4fa6-9681-5455025a7118)


-----------------------------------------------

The current values of these settings can be viewed in the monitor if you search for ssl.

![image](https://github.com/user-attachments/assets/d77e67c4-b2d6-4403-acd5-eb46ff6c0b35)

-----------------------------------------------

It is recomended that you setup a node graph that set these values when you start VNyan and also change them through a node graph if you change the scene.

Here is a example where all the settings are changed. I can provide a node graph example if anybody wants this.
![image](https://i.imgur.com/XU8hFt8.png)

