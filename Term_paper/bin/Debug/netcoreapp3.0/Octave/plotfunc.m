clc;
clear;
k=0:0.01:pi/2;
index=1;
for y=0:0.01:pi/2;
   t(index)=((-0.524)*y^0 + (0.073*y^1) + (0.238*y^2) + (0.494*y^3) + (-0.502*y^4) + (-0.01*y^5) + (-0.027*y^6) + (0.038*y^7) + (0.128*y^8) + (-0.197*y^9));
   index=index+1;
endfor
hold on;

plot(k,cos(k))
plot(k,t,".")
