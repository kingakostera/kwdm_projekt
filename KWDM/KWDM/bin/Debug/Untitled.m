listing = dir('C:\Users\Kinga\Desktop\KWDM\KWDM\KWDM\bin\Debug\Baza\LGG-104\1');
dicom_files_list = listing(arrayfun(@(x) ~strcmp(x.name(1),'.'),listing));
obraz = dicomread([dicom_files_list(1).name]);
info = dicominfo([dicom_files_list(1).name]);
