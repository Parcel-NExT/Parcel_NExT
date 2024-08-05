# Parcel Dashboard App

Tags: #Experimental

Quick dahbosard bulding based on existing service.

## Technical Notes

Architecture: Because Streamlit cannot be self-hosted within a python script (it requires triggering the entry script and an isolated python entrance anyway), we will just use CodeGen for the script; We will use Pythonnet for interoperation with existing .Net infrastructure but because streamlit is in general a completely separate service, there will be serialization needs and keep the calling process (Parcel) and the web app (Streamlit) separate.

We could serve data using web services from wihtin parcel but it's cleaner and ideal to have dashbaord running when parcel is closed so we will just save a copy of all needed data in the streaming folder.