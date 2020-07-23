#!/usr/bin/env bash

if [ ! ${APPCENTER_SOURCE_DIRECTORY} ]
then
    echo "Скрипт запущен локально"
    if [ ! $1 ] && [ ! $2 ]
        then 
            echo -e "Нет аргумента для задания имени пакета, отмена.\nИспользование: sh <скрипт> <имя пакета> <имя приложения>"
            exit 1
    fi

    PACKAGENAME=$1
    LABEL=$2
    ACTIVITY=MainActivity.cs
    MANIFEST=Properties/AndroidManifest.xml
    ORIGINAL_GS=google-services.json
    GS_PATH=google-services_${PACKAGENAME//"."/"_"}.json
    ORIGINAL_ICON=Resources/drawable/icon_login.png 
    ICON_PATH=Resources/drawable/icon_login_${PACKAGENAME//"."/"_"}.png 
else
    echo "## [section][Pre-Build] Setting up Environment variables"
    echo "## [section][Pre-Build] Getting data from variable PACKAGE_NAME"
    PACKAGENAME=${PACKAGE_NAME}
    if [ ! ${PACKAGENAME} ]
    then
        echo "## [section][Pre-Build] No package name provided. Aborting"
        exit 1
    fi
    echo "## [section][Pre-Build] Getting data from variable LABEL_NAME"
    LABEL=${LABEL_NAME}
    if [ ! ${LABEL} ]
    then
        echo "## [section][Pre-Build] No label provided. Aborting"
        exit 1
    fi

    ROOT=${APPCENTER_SOURCE_DIRECTORY}/xamarinJKH/xamarinJKH.Android/

    ACTIVITY=${ROOT}/MainActivity.cs
    MANIFEST=${ROOT}/Properties/AndroidManifest.xml
    ORIGINAL_ICON=${ROOT}/Resources/drawable/icon_login.png 
    ICON_PATH=${ROOT}/Resources/drawable/icon_login_${PACKAGENAME//"."/"_"}.png 
    ORIGINAL_GS=${ROOT}/google-services.json
    GS_PATH=${ROOT}/google-services_${PACKAGENAME//"."/"_"}.json
fi

# PACKAGENAME="sys_rom.ru.vodokanal2"
# LABEL="Водоканал"
if [ ${#LABEL} -gt 0 ]
 then
    if [ -a ${ACTIVITY} ]; then
        sed -i "s/Label = \"[-a-zA-Z0-9 | _ | ' ' | А-Яа-я]*\"/Label = \"${LABEL}\"/" ${ACTIVITY}
        echo "## [section][Pre-Build] Label changed";
    else 
        echo ERROR: "## [section][Pre-Build] File MainActivity.cs not found. Check the path, aborting"
        exit 1
    fi
    else 
        echo ERROR: "## [section][Pre-Build] Label is not set, this change will not apply. Aborting"
        exit 1
fi
if [ ${#PACKAGENAME} -gt 0 ]
 then
 if [ -a ${MANIFEST} ]
 then
    echo "## [section][Pre-Build] Setting up Package name"
    sed -i "s/android:label=\"[-a-zA-Z0-9а-яА-Я | ' ']*\"/android:label=\"${LABEL}\"/" ${MANIFEST}
    sed -i "s/package=\"[a-z0-9 | . | _]*\"/package=\"${PACKAGENAME}\"/" ${MANIFEST}
 fi
    
    if [  -a ${ICON_PATH} ]
    then
        cat ${ICON_PATH} > ${ORIGINAL_ICON}
        echo "## [section][Pre-Build] Icon data is copied to original icon"
    else
        echo ERROR: "## [section][Pre-Build] Icon file is not found, this change will not apply. Aborting"
        exit 1
    fi

    if [ -a ${GS_PATH} ]
    then
        cat ${GS_PATH} > ${ORIGINAL_GS}
        echo "## [section][Pre-Build] Google Services file has been changed to one for ${PACKAGENAME}"
    else
        echo ERROR: "## [section][Pre-Build] Google Services for ${PACKAGENAME} not found. Aborting"
        exit 1
    fi

fi