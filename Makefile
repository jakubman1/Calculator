.PHONY: all pack clean run help

path = src/bin/Debug/
exec = Calculator.exe

all: run

pack:
	zip -r archive.zip *

clean:
	find . * -not -name Makefile -not -name *.zip -delete

run:
	$(path)$(exec)

help:
	echo "Program by mel jit bez problemu spustit, pokud narazite na potize, zkontrolujte prosim ze mate nainstalovany aktualni Microsoft .NET Framework (verze 4.6 nebo novejsi)."
