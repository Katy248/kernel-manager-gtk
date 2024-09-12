APP_NAME := kernel-manager-gtk
APP_ID := ru.katy248.$(APP_NAME)

PROJECT_DIR := ./$(APP_NAME)

.PHONY: run build publish run-publish install clear clean pot-file

DOTNET_FLAGS = --configuration=Release
# env variables
export LANG=ru_RU.utf-8

#
run: DOTNET_FLAGS += --property DefineConstants=RED_OS
run: build
	echo "Note that I will use sudo!" >&2
	# GDK_SYNCHRONIZE=1
	sudo dotnet ./build/$(APP_NAME).dll

run-arch: DOTNET_FLAGS += --property DefineConstants=ARCH_LINUX
run-arch: build
	dotnet ./build/$(APP_NAME).dll

build:
	@echo "Build flags: '$(DOTNET_FLAGS)'"
	dotnet build $(PROJECT_DIR)/$(APP_NAME).csproj --output ./build $(DOTNET_FLAGS)

run-publish: publish
	./publish/kernel-manager-gtk

publish:
	@echo "Build flags: '$(DOTNET_FLAGS)'"
	dotnet publish -o "./publish" --no-self-contained --ucr $(DOTNET_FLAGS)

SOURCES := $(wildcard $(PROJECT_DIR)/*.cs)
PO_FILES := $(wildcard ./po/*.po)
MO_FILES := $(patsubst ./po/%.po, ./mo/%.mo, $(PO_FILES))
INSTALLED_MO_FILES := $(patsubst ./po/%.po, /usr/share/locale/%/LC_MESSAGES/$(APP_ID).mo, $(PO_FILES))

./mo/%.mo: ./po/%.po
	mkdir -p ./mo
	msgfmt $< -o $@

/usr/share/locale/%/LC_MESSAGES/$(APP_ID).mo: ./mo/%.mo 
	mkdir -p `dirname $@`
	cp $< $@

install: $(INSTALLED_MO_FILES)

pot-file:
	xgettext $(SOURCES) \
		-o ./po/messages.pot \
		--language=c# \
		--from-code=utf-8

	msgmerge -U ./po/ru.po ./po/messages.pot

clear clean:
	rm -rf $(PROJECT_DIR)/bin
	rm -rf $(PROJECT_DIR)/obj

