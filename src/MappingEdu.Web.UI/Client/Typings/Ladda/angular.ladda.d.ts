
declare module ladda {

    interface ILaddaOptions {
        style: string
    }

    interface ILaddaProvider {
        setOption(option: ILaddaOptions): void
    }
}
