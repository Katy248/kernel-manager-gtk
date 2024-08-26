APP_ID := ru.katy248.kernel-manager-gtk

.PHONY: run publish run-release install

run:
	echo "Note that I will use sudo!" >&2
	# GDK_SYNCHRONIZE=1 
	sudo dotnet run --project kernel-manager-gtk/kernel-manager-gtk.csproj

run-release: publish
	./publish/kernel-manager-gtk

publish:
	dotnet publish -c Release --no-self-contained --ucr -o "./publish"


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

