# Build

## on Windows

Open the *Kephas.sln* Solution in Visual Studio and click on build.

If you don't have the Visual Studio, you can download the free „[community edition](https://www.visualstudio.com/de-de/visual-studio-homepage-vs.aspx)“.

Or you can use vagrant like on Mac/Linux (see below).



## on Mac and Linux

### over vagrant

When you don't have a mono-development environment, you can easily setup a virtual machine for development over vagrant.

The only dependency, who you need is [vagrant](https://www.vagrantup.com/), all other tools was installed in a virtual machine.

* startup virtual machine `vagrant up`
* login `vagrant ssh`
* goto the source-directory `cd /vagrant/src`
* and run the build with `nant`


### native

if you have already a mono development environment installed, you can just call `nant` into the *src*-directory.