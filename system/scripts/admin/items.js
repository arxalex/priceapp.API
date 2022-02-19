Vue.component('Items', {
    template: `
        <div v-on:itemsaver="itemsaverActivate">
            <itemsaver :sourceItem="itemsaver.sourceItem"
                :destinationItem="itemsaver.destinationItem"
                v-if="itemsaver.saveActive"></itemsaver>
            <h1>Prepare items</h1>
            <p>List:
                <div v-for="shop in shops">
                    <span>{{ shop.id }}</span> - <span>{{ shop.label }}</span>
                </div>
            </p>
            <div class="input-group mb-3">
                <select class="form-select" v-model="selectedShopId">
                    <option selected disabled>Chose shop</option>
                    <option v-for="shop in shops" v-bind:value="shop.id">{{ shop.label }}</option>
                </select>
                <select class="form-select" v-model="selectedCategoryId">
                    <option selected disabled>Chose category</option>
                    <option v-for="shopCategory in shopCategories" v-bind:value="shopCategory.id">{{ shopCategory.label }}</option>
                </select>
            </div>
            <div class="input-group mb-3">
                <button @click="get_items()" class="btn btn-primary mt-2 w-100">Get items</button>
            </div>
            <div class="col-sm-12">
                <item class="mb-3" 
                    v-for="itemModel in itemModels" 
                    :item="itemModel.item" 
                    :similarItems="itemModel.similarItems">
                </item>
            </div>
            <button class="btn btn-secondary" @click="get_prev()" :disabled="page.isPrevDisabled">Prev</button>
            <button class="btn btn-primary" @click="get_next()" :disabled="page.isNextDisabled">Next</button>
        </div>
    `,
    data() {
        return {
            shops: {
                silpo: {
                    label: 'silpo',
                    id: 1,
                },
                atb: {
                    label: 'atb',
                    id: 3,
                },
                auchan: {
                    label: 'auchan',
                    id: 2,
                }
            },
            selectedShopId: null,
            selectedCategoryId: null,
            itemModels: [],
            shopCategories: [],
            page: {
                from: 0,
                to: 25,
                isPrevDisabled: true,
                isNextDisabled: false,
            }
        }
    },
    methods: {
        get_categories: async function () {
            const categoriesUrl = "../be/categories/get_categories";
            var shop = this.selectedShopId;
            var data = {
                source: shop
            };
            var categories = await this.getItemsFromDb(categoriesUrl, data);
            this.shopCategories = categories;
        },
        get_items: async function () {
            this.itemModels = [];
            const similarUrl = "../be/items/get_similar_items";
            const getItemUrl = "../be/items/get_items";

            var shop = this.selectedShopId;
            var category = this.selectedCategoryId;

            var data = {
                source: shop,
                category: category,
                from: this.page.from,
                to: this.page.to
            };

            var items = await this.getItemsFromDb(getItemUrl, data);

            var itemLabels = [];
            items.forEach(element => {
                itemLabels.push(element.label);
            });
            var similarItems = await this.getItemsFromDb(similarUrl, {
                itemLabels: itemLabels
            });

            for (var i = 0; i < items.length; i++) {
                this.itemModels.push({
                    item: items[i],
                    similarItems: similarItems[i]
                });
            }
        },
        get_prev: function () {
            if (this.page.from >= 25 && !this.page.isPrevDisabled) {
                this.page.from -= 25;
                this.page.to -= 25;
                this.get_items();
                if (this.page.from <= 0) {
                    this.page.isPrevDisabled = true;
                }
            }
        },
        get_next: function () {
            if (this.page.to <= 9975 && !this.page.isNextDisabled) {
                this.page.from += 25;
                this.page.to += 25;
                this.get_items();
                if (this.page.to >= 10000) {
                    this.page.isNextDisabled = true;
                }
            }
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        itemsaverActivate: function () {
            
        }
    },
    watch: {
        selectedShopId: function (value) {
            this.get_categories();
        }
    },
    async mounted() {
        const labelsUrl = "../be/items/get_labels";
        var labels = await this.getItemsFromDb(labelsUrl, {
            method: "GetAllLabels"
        });
        Vue.prototype.$labels = labels;
        Vue.prototype.$itemsaver = {
            sourceItem: null,
            destinationItem: null,
            saveActive: false
        }
    },
    computed: {
        itemsaver: function() {
            return this.$itemsaver;
        }
    }
});

