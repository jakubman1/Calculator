.PHONY: all pack clean run help

path = src/bin/Debug/
exec = Calculator.exe
dirname = xtykva00_xmanja00_xslesi02_xholas09


all: run

pack:
	cd ../../.. && find . $(dirname) && zip -r $(dirname).zip $(dirname)

clean:
	cd ../../.. && rm -r $(dirname)

run:
	$(path)$(exec)

help:
	echo "Program by mel jit bez problemu spustit, pokud narazite na potize, zkontrolujte prosim ze mate nainstalovany aktualni Microsoft .NET Framework (verze 4.6 nebo novejsi)."
