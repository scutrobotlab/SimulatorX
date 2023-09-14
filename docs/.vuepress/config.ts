import {defineUserConfig} from "vuepress-vite";
import {hopeTheme, mdEnhance} from "vuepress-theme-hope";
import {gitPlugin} from "@vuepress/plugin-git";
import {searchProPlugin} from "vuepress-plugin-search-pro";
import {mdEnhancePlugin} from "vuepress-plugin-md-enhance";

export default defineUserConfig({
    theme: hopeTheme({
        author: {
            name: "华南虎 模拟器组",
            url: "https://sim.scutbot.cn",
            email: "sim@scutbot.cn"
        },
        favicon: "/static/images/icon.png",
        repo: "https://github.com/scutrobotlab/SimulatorX",
        navbarAutoHide: "mobile",
        docsBranch: "pages",
        displayFooter: true,
        darkmode: "enable",
        navbar: [
            {
                prefix: "/guide/",
                text: "快速上手",
                icon: "icon-park-twotone:guide-board",
                children: ["ui.md", "control.md", "game.md", "install.md"]
            },
            {
                prefix: "/updates/",
                text: "新版升级",
                icon: "clarity:new-solid",
                children: ["scene.md", "roles.md", "rules.md", "interacts.md"]
            },
            {
                prefix: "/influences/",
                text: "影响力",
                icon: "tdesign:cooperate",
                children: ["matches/README.md"]
            },
            {
                prefix: "/publicize/",
                text: "宣传工作",
                icon: "arcticons:ad-free",
                children: ["ba.md", "bl.md", "bv.md", "wa.md"]
            },
            {
                prefix: "/architecture/",
                text: "软件架构",
                icon: "ic:round-design-services",
                children: ["visual.md", "network.md", "vehicle-sim.md", "model.md", "workflow.md"]
            },
            {
                link: "/future.md",
                text: "展望未来",
                icon: "arcticons:dreamer"
            },
            {
                link: "/about.md",
                text: "关于",
                icon: "mdi:about",
            },
        ],
        navbarLayout: {
            start: ["Links"],
            center: ["Brand", "Repo"],
            end: ["Search"]
        },
        iconAssets: "iconify"
    }),
    title: "SimulatorX",
    locales: {
        "/": {
            lang: "zh-CN",
        }
    },
    plugins: [
        gitPlugin({}),
        searchProPlugin({
            indexContent: true,
            customFields: [
                {
                    // @ts-ignore
                    getter: (page) => page.frontmatter.category,
                    formatter: "分类：$content",
                },
                {
                    // @ts-ignore
                    getter: (page) => page.frontmatter.tag,
                    formatter: "标签：$content",
                },
            ],
        }),
        mdEnhancePlugin({
            card: true,
            container: true
        })
    ],
})