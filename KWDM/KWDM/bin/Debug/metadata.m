function [ patientName, patientID, patientSex, patientAge, patientWeight, studyDate, studyTime,...
    modality, studyDescription, seriesDate, seriesTime, seriesDescription] = metadata( path )

addpath(path) 
listing = dir(path);
dicom_files_list = listing(arrayfun(@(x) ~strcmp(x.name(1),'.'),listing));
info = dicominfo([path '\\' dicom_files_list(1).name]);
patientName = info.PatientName.FamilyName;
patientID = info.PatientID;
patientSex = info.PatientSex;
patientAge = info.PatientAge;
patientWeight = info.PatientWeight;
studyDate = info.StudyDate;
studyTime = info.StudyTime;
modality = info.Modality;
studyDescription = info.StudyDescription;
seriesTime = info.SeriesTime;
seriesDate = info.SeriesDate;
seriesDescription = info.SeriesDescription;
end

