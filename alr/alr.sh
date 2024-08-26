export name='kernel-manager-gtk-git'
export version='0.0.1'
export trelease='beta'
export desc=''
export license=('none' 'dwtfyw')


export architectures=('all')
export provides=('kernel-manager-gtk')
export conflicts=('kernel-manager-gtk-git' 'kernel-manager-gtk')
export build_deps=('dotnet' 'libadwaita')
export deps=('dotnet' 'libadwaita')
export sources=('https://github.com/katy248/kernel-manager-gtk.git')
export checksums=('SKIP')

build() {
    cd "${srcdir}/kernel-manager-gtk" || exit 1
    dotnet build -c Release --no-self-contained --ucr
}

package() {
    install -Dm755 "${srcdir}/kernel-manager-gtk/bin/Release/net8.0/linux-x64/" "/usr/share/kernel-manager-gtk/"
    install -Dm755 "${srcdir}/kernel-manager-gtk.sh" "/usr/bin/kernel-manager-gtk"
}
