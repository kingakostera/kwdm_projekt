function [  ] = manualSegmentation( path )
addpath(path);
listing = dir(path);
dicom_files_list = listing(arrayfun(@(x) ~strcmp(x.name(1),'.'),listing));
obraz = dicomread([path '\\' dicom_files_list(1).name]);
% filtr medianowy
v_filt = medfilt2(obraz);
figure(1)
imshow(v_filt, [])
obrys = imfreehand;
guz = createMask(obrys);

figure(2)
imshow(guz);

L = bwlabel(guz);
RP = regionprops(L, 'Area');
RP = struct2array(RP);
[A, idx]= max(RP);
L(L~=idx) = 0;
L(L~=0) = 1;

figure(2)
imshow(L)

folder_name = 'wyniki_segmentacji_manualnej';
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
    saveas(figure(1),[path '\' folder_name '\' listing(length(listing)).name(1:end-5) num2str((length(listing))) '.png']);
else
    mkdir(path, folder_name);
    saveas(figure(1),[path '\' folder_name '\' dicom_files_list(1).name(1:end-4) '_obrys_0.png']);
    %saveas(figure(2),[path '\' folder_name '\' dicom_files_list(1).name(1:end-4) '_0.png']);
end

close all
end

