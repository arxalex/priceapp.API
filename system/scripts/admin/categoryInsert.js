Vue.component('categoryInsert', {
    template: `
        <div class="bg-white position-fixed p-4 window-insert shadow-lg rounded">
            <h5 class="ms-1 fw-bold mb-3">Category</h5>
            <div class="d-flex mb-3">
                <span>{{ category.id }}</span>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Label</label>
                    <input class="form-control" v-model="category.label" placeholder="Label">
                </div>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Parent</label>
                    <select class="form-select" v-model="category.parent">
                        <option disabled value="">Chose parent category</option>
                        <option v-for="categoryL in categories" :value="categoryL.id">{{ categoryL.label }}</option>
                    </select>
                </div>
                <div class="position-relative mx-2 mt-2">
                    <label class="fw-light fs-min bg-white position-absolute input-label">isFilter</label>
                    <input class="form-check-input mx-2" type="checkbox" v-model="category.isFilter">
                </div>
            </div>
            <h5 class="ms-1 fw-bold mb-3">Category link</h5>
            <div class="d-flex mb-3">
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Id</label>
                    <input class="form-control" v-model="categoryLink.id" placeholder="Id">
                </div>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Label</label>
                    <select class="form-select" v-model="categoryLink.categoryid">
                        <option disabled>Chose category</option>
                        <option :value="null">Null</option>
                        <option v-for="categoryL in categories" :value="categoryL.id">{{ categoryL.label }}</option>
                    </select>
                </div>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Shop</label>
                    <select class="form-select" v-model="categoryLink.shopid">
                        <option disabled>Chose shop</option>
                        <option :value="null">Null</option>
                        <option v-for="shop in shops" :value="shop.id">{{ shop.label }}</option>
                    </select>
                </div>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Category shop id</label>
                    <input class="form-control" v-model="categoryLink.categoryshopid">
                </div>
                <div class="position-relative flex-fill">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Shop category label</label>
                    <input class="form-control" v-model="categoryLink.shopcategorylabel">
                </div>
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

