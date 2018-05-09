function [  ] = automaticalSegmentation( path )
addpath(path);
listing = dir(path);
dicom_files_list = listing(arrayfun(@(x) ~strcmp(x.name(1),'.'),listing));
obraz = dicomread([path '\\' dicom_files_list(1).name]);
% filtr medianowy
v_filt = medfilt2(obraz);

figure(1)
imshow(v_filt, [])
hold on
[x1,y1] = ginput(1);
x1 = round(x1);   
y1 = round(y1);

[w,k] = size(v_filt);
v_wynik = zeros(w,k);

temp = zeros(w,k);
Z = v_filt(y1-3:y1+3,x1-3:x1+3);
z = mean(mean(Z));
max_dJ = (0.25*z);
punkty = [y1; x1];
temp(y1,x1) = 1;
v_wynik(y1,x1) = 1;

while(~isempty(punkty))
    obecny = punkty(:,1);
    punkty = punkty(:,2:size(punkty,2));
    sasiedzi = [obecny(1), obecny(1) - 1, obecny(1)+1,obecny(1);
    obecny(2)-1, obecny(2), obecny(2),obecny(2)+1];
    for i = 1:4
        d = sasiedzi(:,i);
        if(d(1,1)>0 && d(1,1)<=w && d(2,1)>0 && d(2,1)<=k)
            if temp(d(1,1),d(2,1)) == 0
                temp(d(1,1), d(2,1)) =1;
                if(v_filt(d(1,1),d(2,1)) >= z-max_dJ && v_filt(d(1,1),d(2,1)) <= z+max_dJ)
                    v_wynik(d(1,1),d(2,1)) = 1;
                    punkty = [punkty, d];
                end
            end
        end
    end
end

v2 = [1 1 1 1 1; 1 1 1 1 1; 1 0 0 1 1; 1 1 1 1 1; 1 1 0 0 1];
v_wynik = imopen(imclose(v_wynik,v2), v2);

v_wynik = imfill(v_wynik, 'holes');

%Vwynikowe=imopen(Vwynikowe, V2);

L = bwlabel(v_wynik);
RP = regionprops(L, 'Area');
RP = struct2array(RP);
[A, idx] = max(RP);
L(L~=idx) = 0;
L(L~=0) = 1;

figure(2)
imshow(L)

% rysowanie konturu na obrazie poczatkowym
B = bwboundaries(L);
figure(3)
imshow(v_filt, []); 
hold on
for k = 1 : length(B)
    b = B{k};
    plot(b(:,2),b(:,1),'r','linewidth',2);
end
folder_name = 'wyniki_segmentacji_automatycznej';
listing = dir([path '\' folder_name]);
listing = listing(arrayfun(@(x) ~strcmp(x.name(1),'.'),listing));

if(length(listing)>0)
    full_path = [path '\' folder_name];
    dir_info = dir(full_path);
    if(length(dir_info)>2)
        dir_info([dir_info.isdir]) = [];
        filenames = fullfile(full_path,{dir_info.name});
        %delete(filenames{:})
    end
    dicom_files_list(1).name(end-5:end-4)
    %saveas(figure(2),[path '\' folder_name '\' listing(1).name(1:end-5) num2str((length(listing))/2) '.png']);
    saveas(figure(3),[path '\' folder_name '\' listing(length(listing)).name(1:end-5) num2str((length(listing))) '.png']);
else
    mkdir(path, folder_name);
    saveas(figure(3),[path '\' folder_name '\' dicom_files_list(1).name(1:end-4) '_obrys_0.png']);
    %saveas(figure(2),[path '\' folder_name '\' dicom_files_list(1).name(1:end-4) '_0.png']);
end



close all
end

