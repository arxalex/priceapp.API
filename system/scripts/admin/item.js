Vue.component('item', {
    template: `
        <div class="row">
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ item.label }}</h1>
                    Category: {{ item.category }} - {{ itemLabels.categoryLabel }}<br>
                    Brand: {{ item.brand }} - {{ itemLabels.brandLabel }}<br>
                    Package: {{ item.package }} - {{ itemLabels.packageLabel }}, units: {{ item.units }} {{ itemLabels.packageShort }}, term: {{ item.term }}<br>
                    Barcodes: <span v-for="barcode in item.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in item.consist">{{ consistId }}, </span> - <span v-for="consistLabel in itemLabels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ item.calorie }}, carbohydrates: {{ item.carbohydrates }}, fat: {{ item.fat }}, proteins: {{ item.proteins }}<br>
                    Country: {{ item.additional.country }} - {{ itemLabels.countryLabel }} <br>
                </div>
            </div>
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="currentSimilarItem.image">
                </div>
                <div class="col-sm-8" v-if="currentSimilarItem !== null">
                    <h1>{{ currentSimilarItem.label }}</h1>
                    Category: {{ currentSimilarItem.category }} - {{ currentSimilarLabels.categoryLabel }}<br>
                    Brand: {{ currentSimilarItem.brand }} - {{ currentSimilarLabels.brandLabel }}<br>
                    Package: {{ currentSimilarItem.package }} - {{ currentSimilarLabels.packageLabel }}, units: {{ currentSimilarItem.units }} {{ currentSimilarLabels.packageShort }}, term: {{ currentSimilarItem.term }}<br>
                    Barcodes: <span v-for="barcode in currentSimilarItem.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in currentSimilarItem.consist">{{ consistId }}, </span> - <span v-for="consistLabel in currentSimilarLabels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ currentSimilarItem.calorie }}, carbohydrates: {{ currentSimilarItem.carbohydrates }}, fat: {{ currentSimilarItem.fat }}, proteins: {{ currentSimilarItem.proteins }}<br>
                    Country: {{ currentSimilarItem.additional.country }} - {{ currentSimilarLabels.countryLabel }}<br>
                </div>
                <div class="">
                    <select class="form-select" v-model="currentSimilarId">
                        <option selected disabled>Chose opinion</option>
                        <option name="item" value="0">New item</option>
                        <option v-for="similarOption in similarOptions" v-bind:value="similarOption.value">{{ similarOption.text }}</option>
                    </select>
                    <button class="btn btn-primary" @click="insert">Insert</button>
                </div>

            </div>
        </div>
    `,
    data() {
        return {
            similarLabels: [],
            currentSimilarItem: null,
            currentSimilarLabels: null,
            currentSimilarId: 0,
            similarOptions: [],
            saveDisabled: false,
            itemLabels: null
        }
    },
    mounted() {
        var i = 1;
        var itemCategoryLabels = this.$labels.categories.filter(value => value.id == item.category);
        var itemBrandLabels = this.$labels.brands.filter(value => value.id == item.brand);
        var itemPackageLabels = this.$labels.packages.filter(value => value.id == item.package);
        var itemConsistLabels = this.$labels.consists.filter(value => item.consist.includes(value.id));
        var itemCountryLabels = this.$labels.countries.filter(value => value.id == item.country);
        this.itemLabels = {
            categoryLabel: itemCategoryLabels.length >= 0 ? itemCategoryLabels[0].label : null,
            brandLabel: itemBrandLabels.length >= 0 ? itemBrandLabels[0].label : null,
            packageLabel: itemPackageLabels.length >= 0 ? itemPackageLabels[0].label : null,
            packageShort: itemPackageLabels.length >= 0 ? itemPackageLabels[0].short : null,
            consistLabels: itemConsistLabels.length >= 0 ? itemConsistLabels.map(value => value.label) : [],
            countryLabel: itemCountryLabels.length >= 0 ? itemCountryLabels[0].label : null
        };

        this.similarLabels = this.similarItems.map(similarItem => {
            var categoryLabels = this.$labels.categories.filter(value => value.id == similarItem.category);
            var brandLabels = this.$labels.brands.filter(value => value.id == similarItem.brand);
            var packageLabels = this.$labels.packages.filter(value => value.id == similarItem.package);
            var consistLabels = this.$labels.consists.filter(value => similarItem.consist.includes(value.id));
            var countryLabels = this.$labels.countries.filter(value => value.id == similarItem.country);
            return {
                categoryLabel: categoryLabels.length >= 0 ? categoryLabels[0].label : null,
                brandLabel: brandLabels.length >= 0 ? brandLabels[0].label : null,
                packageLabel: packageLabels.length >= 0 ? packageLabels[0].label : null,
                packageShort: packageLabels.length >= 0 ? packageLabels[0].short : null,
                consistLabels: consistLabels.length >= 0 ? consistLabels.map(value => value.label) : [],
                countryLabel: countryLabels.length >= 0 ? countryLabels[0].label : null
            };
        });
        this.similarItems.forEach(element => {
            this.similarOptions.push({ text: element.label, value: i++ });
        });
    },
    methods: {
        change: function () {
            if (this.currentSimilarId == 0) {
                this.insertToDB();
            } else {
                this.currentSimilarItem = this.similarItems[this.currentSimilarId - 1];
                this.currentSimilarLabels = this.similarLabels[this.currentSimilarId - 1];
            }
        },
        insert: function () {
            this.saveDisabled = true;
        }
    },
    props: {
        item: {
            type: Object
        },
        similarItems: {
            type: Array
        },
    },
    computed: {
    },
    watch: {
        currentSimilarId: function (value) {
            this.change();
        }
    }
});