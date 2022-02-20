Vue.component('categoryInsert', {
    template: `
        <div class="bg-white position-fixed p-4 window-insert shadow-lg rounded">
            <div class="input-group mb-3">
                <span>{{ category.id }}</span>
                <input class="form-control" v-model="category.label" placeholder="Label">
                <select v-model="category.parent">
                    <option disabled value="">Chose parent category</option>
                    <option v-for="categoryL in categories" :value="categoryL.id">{{ categoryL.label }}</option>
                </select>
                <div class="form-check m-2">
                    <input class="form-check-input" type="checkbox" v-model="category.isFilter">
                    <label class="form-check-label">isFilter</label>
                </div>
            </div>
            <div class="row mb-3">
                <input class="form-control" v-model="categoryLink.id" placeholder="Id">
                <select v-model="categoryLink.categoryid">
                    <option disabled>Chose category</option>
                    <option :value="null">Null</option>
                    <option v-for="categoryL in categories" :value="categoryL.id">{{ categoryL.label }}</option>
                </select>
                <select v-model="categoryLink.shopid">
                    <option disabled>Chose shop</option>
                    <option :value="null">Null</option>
                    <option v-for="shop in shops" :value="shop.id">{{ shop.label }}</option>
                </select>
                <input class="form-control" v-model="categoryLink.categoryshopid" placeholder="Category shop id">
                <input class="form-control" v-model="categoryLink.shopcategorylabel" placeholder="Category shop id">
            </div>
            <div class="input-group">
                <button class="btn btn-primary form-control" v-on:click='insertCategory'>Insert</button>
            </div>
        </div>
    `,
    data() {
        return {
            category: {
                id: null,
                label: this.sourceCategory,
                parent: null,
                isFilter: true
            },
            categoryLink: {
                id: null,
                categoryid: null,
                shopid: null,
                categoryshopid: null,
                shopcategorylabel: null
            }
        }
    },
    props: {
        sourceCategory: {
            type: String
        },
    },
    methods: {
        insertCategory: async function () {
            const insertUrl = "../be/categories/insert_categories";
            await this.getItemsFromDb(insertUrl, {
                method: "InsertToCategoriesAndUpdateLink",
                category: this.category,
                category_link: this.categoryLink
            });
            const labelsUrl = "../be/items/get_labels";
            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("categoryInserted");
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
    },
    async mounted() {
        const cateoryLinkUrl = "../be/categories/get_categories";
        var categoryLink = await this.getItemsFromDb(cateoryLinkUrl, {
            method: "GetCategoryLinkByLabel",
            label: this.sourceCategory
        });
        this.categoryLink = categoryLink;
    },
    computed: {
        categories: {
            cache: false,
            get: function () {
                return this.$labels.categories;
            }
        },
        shops: function() {
            return this.$labels.shops;
        }
    }
});

