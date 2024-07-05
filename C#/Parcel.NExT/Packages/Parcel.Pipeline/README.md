# Parcel.Pipeline

Provide streamlined process management, specifically for CLI usage. Run CLI commands perl style. Streamlines running command and gets output as string. A cool feature (for API use) is the chaining operator `|` that provides Elixir like data transformation syntax. Notice this package must be AOT. A nice thing about this package is that it's very easy to embed (besides being AOT and depends on no fancy .Net language features other than basic operating system IO and process management) - compared to the much more bulky PowerShell language.

## Usages

### API Usage

Provides:

* `string Run(string, string)`
* `string Run(string, string[])`
* `string Run(string, Dictionary<string, string>)`

## Shell Usage ($PSL)

PENDING DOC.

### Pseudo-Linux commands

Provided for portability.

### Pseudo-PowerShell commands

Counterparts for Pseudo-Linux commands.

* Set-Location

### Limits

* Variable interpolation in the middle of argument is not supported yet.

## TODO

- [ ] `Run` should return a `Session` object instead and provide Fluent API to chain program runs together and allow piping inputs/ouputs. The session object may provide "Result" for the last run result instead of returning a string output directly from `Run()`, and optionally save all intermediate results inside the Session object.

## Changelog

Versions:

* v0.0.1/v0.1.0: (Pure) Pre-Alpha.
* v0.2.0: (Pure) Support standard input redirect from string; Implement Pipeline Fluent API.
* v0.3.0: (Pure) Implement chaining/pipeline operator `|`; Add doc.
* v0.4.0: (Parcel NExT) $PSL support.