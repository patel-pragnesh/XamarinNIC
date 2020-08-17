#!/usr/bin/env bash

if [ ! ${APPCENTER_SOURCE_DIRECTORY} ]
then
    echo "Скрипт запущен локально"
    if [ ! $1 ] && [ ! $2 ] && [ ! $3 ] && [ ! $4 ]
        then 
            echo -e "Нет аргумента для задания имени пакета, отмена.\nИспользование: sh <скрипт> <имя пакета> <имя приложения> <версия> <база>"
            exit 1
    fi

    PACKAGENAME=$1
    LABEL=$2
    VERSION=$3
    BASE=$4
    ACTIVITY=MainActivity.cs
    MANIFEST=Properties/AndroidManifest.xml
    CLIENT_SCRIPT=../xamarinJKH/Server/RestClientMP.cs
    ORIGINAL_GS=google-services.json
    GS_PATH=google-services_${PACKAGENAME//"."/"_"}.json
    ORIGINAL_ICON=Resources/drawable/icon_login.png 
    ICON_PATH=Resources/drawable/icon_login_${PACKAGENAME//"."/"_"}.png 
else
    echo "##[section][Pre-Build] Setting up Environment variables"
    echo "##[section][Pre-Build] Getting data from variable PACKAGE_NAME"
    PACKAGENAME=${PACKAGE_NAME}
    if [ ! ${PACKAGENAME} ]
    then
        echo "##[section][Pre-Build] No package name provided. Aborting"
        exit 1
    fi
    echo "##[section][Pre-Build] Getting data from variable LABEL_NAME"
    LABEL=${LABEL_NAME}
    if [ ! ${LABEL} ]
    then
        echo "##[section][Pre-Build] No label provided. Aborting"
        exit 1
    fi

    echo "##[section][Pre-Build] Getting data from variable VERSION_BUILD"
    VERSION=${VERSION_BUILD}
    if [ ! ${VERSION}]; then
        echo "##[section][Pre-Build] No version provided. Aborting"
        exit 1
    fi

    echo "##[section][Pre-Build] Getting data from variable DATABASE"
    BASE=${DATABASE}
    if [ ! ${BASE}]; then
        echo "##[section][Pre-Build] No database name provided. Aborting"
        exit 1
    fi

    ROOT=${APPCENTER_SOURCE_DIRECTORY}/xamarinJKH.Android
    CLIENT_SCRIPT=${APPCENTER_SOURCE_DIRECTORY}/xamarinJKH/Server/RestClientMP.cs
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
        sed -i.bak "s/Label = \"[' '| А-Яа-я]*\"/Label = \"${LABEL}\"/"  $ACTIVITY
        rm -f ${ACTIVITY}.bak
        echo "##[section][Pre-Build] Label changed";
        cat ${ACTIVITY}
        echo
    else 
        echo ERROR: "##[section][Pre-Build] File MainActivity.cs not found. Check the path, aborting"
        exit 1
    fi
    else 
        echo ERROR: "##[section][Pre-Build] Label is not set, this change will not apply. Aborting"
        exit 1
fi
if [ ${#PACKAGENAME} -gt 0 ]
 then
 if [ -a ${MANIFEST} ]
 then
    echo "##[section][Pre-Build] Setting up Package name"
    sed -i.bak "s/label=\"[а-яА-Я|' ']*\"/label=\"${LABEL}\"/"  $MANIFEST
    rm -f ${MANIFEST}.bak
    sed -i.bak "s/package=\"[a-z0-9 | . | _]*\"/package=\"${PACKAGENAME}\"/" $MANIFEST
    rm -f ${MANIFEST}.bak
    if [ ${VERSION} ]; then
        echo "##[section][Pre-Build] Setting up version"
        sed -i.bak "s/versionName=\"[0-9|.]*\"/versionName=\"${VERSION}\"/" $MANIFEST
        rm -f ${MANIFEST}.bak
    fi
    cat ${MANIFEST}
 fi
    
    if [  -a ${ICON_PATH} ]
    then
        cat ${ICON_PATH} > ${ORIGINAL_ICON}
        echo "##[section][Pre-Build] Icon data is copied to original icon"
    else
        echo ERROR: "##[section][Pre-Build] Icon file is not found, this change will not apply. Aborting"
        exit 1
    fi

    if [ -a ${GS_PATH} ]
    then
        cat ${GS_PATH} > ${ORIGINAL_GS}
        echo "##[section][Pre-Build] Google Services file has been changed to one for ${PACKAGENAME}"
    else
        echo ERROR: "##[section][Pre-Build] Google Services for ${PACKAGENAME} not found. Aborting"
        exit 1
    fi

fi

if [ ${#CLIENT_SCRIPT} -gt 0 ]; then
    if [ ${BASE} ]; then
        echo "##[section][Pre-Build] Changing database name in ${CLIENT_SCRIPT} to ${BASE}"
        sed -i.bak -e "s/public const string SERVER_ADDR = \"https:\/\/api.sm-center.ru\/[a-z|A-Z|0-9|\.|\/|\:|\-|\_|]*\";/public const string SERVER_ADDR = \"https:\/\/api.sm-center.ru\/${BASE}\";/" $CLIENT_SCRIPT
        rm -f ${CLIENT_SCRIPT}.bak
    else
        echo ERROR: "##[section][Pre-Build] No base variable set. Aborting"
        exit 1
    fi
else
    echo ERROR: "##[section][Pre-Build] No RestClientMP.cs found. Aborting"
    exit 1
fi
echo
echo DONE